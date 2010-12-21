/*
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

*/

#include <stdio.h>
#include <ctype.h>
#include "c3ga.h"

#ifdef WIN32
#define snprintf _snprintf
#pragma warning(disable:4996) /* quit your whining already */
#endif /* WIN32 */


static void c3gaParseWedgeBasisVector(unsigned int bvIdx, float *scale, unsigned int *bitmap) {
	unsigned b;
	
	b = 1 << bvIdx;
	if ((*bitmap) & b) { /* bv ^ bv = 0 */
		*scale = 0.0f;
		return;
	}

	/* add basis vector to bitmap */
	(*bitmap) |= b;

	bvIdx++;
	for (; bvIdx < 5; bvIdx++) /* compute sign flips due to anti commuting basis vectors */
		if ((*bitmap) & (1 << bvIdx)) (*scale) = -(*scale);
}

static void c3gaParseSum(float coord[32], float scale, unsigned int bitmap) {
	int idx = c3ga_basisElementIndexByBitmap[bitmap];
	coord[idx] += scale / (float)c3ga_basisElementSignByIndex[idx];
}

static void c3gaParseDataInit(struct c3gaParseMultivectorData *PD) {
	PD->message[0] = 0;
	mv_setZero(PD->value);
}

/** returns true when 'buf' starts with 'pattern' */
static int c3gaCompareId(const char *pattern, const char *buf, int len) {
	int i = 0;
	while (pattern[i] != 0) {
		if (buf[i] != pattern[i]) return 0;
		else i++;
	}
	if (i != len) return 0; /* must match full pattern */
	else return 1;
}


/** TOKEN IDs: (for internal use only) */
#define T_BAD_IDENTIFIER -100
#define T_BAD_NUMBER -10
#define T_BAD_CHARACTER -1
#define T_END_OF_STRING 0
#define T_WEDGE 1
#define T_MUL 2
#define T_PLUS 3
#define T_MINUS 4
#define T_NUMBER 10
#define T_FIRST_BASIS_VECTOR 100
#define T_LAST_BASIS_VECTOR 104
#define T_NO 100
#define T_E1 101
#define T_E2 102
#define T_E3 103
#define T_NI 104

/**
Returns the next token for the multivector parser.
Skips whitespace.

Sets startIdx and endIdx to start and end (inclusive) of token, except when end of string.

If the next token is a number, anything that atof() can accept should be accepted.

Increments lineIdx when new line char (0x0A) is found, and sets currentLineStart to
the index of that char.
*/
int c3gaParseGetNextToken(const char *buf, int *startIdx, int *endIdx, int *lineIdx, int *currentLineStart) {
	int i, pointFound = 0;

	/* skip all whitespace and other empty stuff, keep track of line index */
	while ((buf[(*startIdx)] > 0) && (buf[(*startIdx)] <= ' ')) {
		if (buf[(*startIdx)] == 0x0A) { /* start of new line */
			(*lineIdx)++;
			*currentLineStart = (*startIdx)+1;
		}
		(*startIdx)++;
	}

	/* detect end of string */
	if (buf[(*startIdx)] == 0) {
		*startIdx = *endIdx;
		return T_END_OF_STRING; /* EOS */
	}

	/* operators */
	if (buf[(*startIdx)] == '^') {*endIdx = *startIdx; return T_WEDGE;} /*   ^   */
	else if (buf[(*startIdx)] == '*') {*endIdx = *startIdx; return T_MUL;} /*   *   */
	else if (buf[(*startIdx)] == '+') {*endIdx = *startIdx; return T_PLUS;} /*   +   */
	else if (buf[(*startIdx)] == '-') {*endIdx = *startIdx; return T_MINUS;} /*   -   */

	else if (isdigit((unsigned char)buf[(*startIdx)]) || (buf[(*startIdx)] == '.')) { /* parse number? */
		*endIdx = (*startIdx);

		/* eat up all digits and at most one point */
		while (isdigit((unsigned char)buf[(*endIdx)]) || (buf[(*endIdx)] == '.')) {
			(*endIdx)++;
			if (buf[(*endIdx)] == '.') {
				(*endIdx)++;
				pointFound = 1;
				break;
			}
		}
		
		if (pointFound) { /* if point found, eat up all digits */
			while (isdigit((unsigned char)buf[(*endIdx)])) {
				(*endIdx)++;
			}
		}

		/* see if there is a 'e' or 'E' */
		if  ((buf[(*endIdx)] == 'e') || (buf[(*endIdx)] == 'E')) {
			(*endIdx)++;
			/* accept at most one +- */
			if  ((buf[(*endIdx)] == '-') || (buf[(*endIdx)] == '+')) {
				(*endIdx)++;
			}

			/* if there is an 'e', there must be some digit */
			if (!isdigit((unsigned char)buf[(*endIdx)])) return T_BAD_NUMBER; /* bad number */

			/* eat up all digits */
			while (isdigit((unsigned char)buf[(*endIdx)])) {
				(*endIdx)++;
			}
		}
		(*endIdx)--; /* end index is inclusive */
		return T_NUMBER;
	}

	else if (isalpha((unsigned char)buf[(*startIdx)]) || (buf[(*startIdx)] == '_')) { /* parse identifier? */
		/* find end of chain of numbers, letters and '_' */
		*endIdx = (*startIdx) + 1;

		while (isalnum((unsigned char)buf[(*endIdx)]) || (buf[(*endIdx)] == '_')) (*endIdx)++;
		(*endIdx)--;  /* end index is inclusive */

		/* see which basis vector it is */
		for (i = 0; i < 5; i++)
			if (c3gaCompareId(c3ga_basisVectorNames[i], buf + (*startIdx), (*endIdx) - (*startIdx) + 1)) return T_FIRST_BASIS_VECTOR + i; /* basis vector */
		return T_BAD_IDENTIFIER; /* bad identifier */
	}

	else return T_BAD_CHARACTER;
} /* end of c3gaParseGetNextToken */

extern void compress(const float *c, float *cc, int *cgu, float epsilon, int gu);
int parse_mvEx(struct c3gaParseMultivectorData *PD, const char *str, const char *strSourceName)
{
	float coord[32];
	char buf[256]; /* used to copy number tokens */
	float scale;
	unsigned int bitmap;
	int startIdx = 0, endIdx;
	int lineIdx = 0, currentLineStart = 0;
	int token, i, beDone, cnt, firstLoop = 1;

	c3gaParseDataInit(PD); /* should reset all */
	c3ga_float_zero_N(coord, 32);

	/* get the first token */
	token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);

	while (1) {
		/* reset for next basis blade */
		bitmap = 0;
		scale = 1.0f;
		beDone = 0;

		if (token == T_END_OF_STRING) break;


		cnt = 0;
		while ((token == T_PLUS) || (token == T_MINUS)) { /* accept all +- */
			cnt++;
			startIdx = endIdx+1;
			if (token == T_MINUS) scale *= -1.0; /*   -   */
			token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);
		}
		
		/* require at least one +- if this is not the first term: */
		if ((!firstLoop) && (cnt == 0)) {
			snprintf(PD->message, 256, "Expected '+' or '-' at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
			return 0;
		}

		if ((token == T_NUMBER) || ((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR))) { /* must be number or basis vector */
			if (token == T_NUMBER) {
				{ /* copy token to buf, multiply scale with value of number */
					for (i = 0; i <= (endIdx-startIdx); i++)
						buf[i] = str[startIdx+i];
					buf[(endIdx-startIdx)+1] = 0;
					scale *= (float)atof((const char*)buf);
				}
				startIdx = endIdx+1;

				/*   * or ^ ?   */
				token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);
				if ((token == T_WEDGE) || (token == T_MUL)) {
					startIdx = endIdx+1;

					/* must find basis vector */
					token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);
				}
				else { /* just a single scalar is OK */
					startIdx = endIdx+1;
					beDone = 1;
				}

			}

			if (!beDone) {
				if ((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR)) {
					c3gaParseWedgeBasisVector((unsigned int)token - T_FIRST_BASIS_VECTOR, &scale, &bitmap);
					startIdx = endIdx+1;
				}
				else {
					snprintf(PD->message, 256, "Expected basis vector at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
					return 0;
				}
			}

			if (!beDone) {
				/* accept ^ basis vector as many times as it takes */
				while (1) {
					/* ^ */
					token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);
					if (token != T_WEDGE) break;
					startIdx = endIdx+1;

					/* basis vector */
					token = c3gaParseGetNextToken(str, &startIdx, &endIdx, &lineIdx, &currentLineStart);
					if ((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR)) {
						c3gaParseWedgeBasisVector((unsigned int)token - T_FIRST_BASIS_VECTOR, &scale, &bitmap);
						startIdx = endIdx+1;
					}
					else {
						snprintf(PD->message, 256, "Expected basis vector at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
						return 0;
					}


				}

			}
		} /* end of 'if number or bv' */
		else if (token == T_BAD_CHARACTER) {
			snprintf(PD->message, 256, "Bad character at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
			return 0;
		}
		else if (token == T_BAD_NUMBER) {
			snprintf(PD->message, 256, "Bad number at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
			return 0;
		}
		else if (token == T_BAD_IDENTIFIER) {
			snprintf(PD->message, 256, "Bad identifier at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
			return 0;
		}
		else {
			snprintf(PD->message, 256, "Unexpected token at %s, line %d, column %d", strSourceName, lineIdx+1, startIdx - currentLineStart +1);
			return 0;
		}

		/* add */
		c3gaParseSum(coord, scale, bitmap);
		
		/* remember that the first loop is done */
		firstLoop = 0;
	}

	/* compress */
	compress(coord, PD->value->c, &(PD->value->gu), 0.0f, 63);

	return 1; /* success */
} /* end of parse_mvEx */

int parse_mv(mv *val, const char *str)
{
	struct c3gaParseMultivectorData PD;
	PD.value = val;
	return parse_mvEx(&PD, str, "");
}


