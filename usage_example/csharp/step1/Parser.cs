using System;
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



namespace c3ga_ns {

	public class Parser {
		private class BasisBlade
		{
			private int bitmap = 0;
			private float scale = 1.0f;

			public int getBitmap() { return bitmap;  }
			public float getScale() { return scale;  }

			public void wedgeBasisVector(int bvIdx)
			{
				int b;

				b = 1 << bvIdx;
				if ((bitmap & b) != 0)
				{ // bv ^ bv = 0
					scale = 0.0f;
					return;
				}
				else
				{
					// add basis vector to bitmap	
					bitmap |= b;

					bvIdx++;
					for (; bvIdx <= 5; bvIdx++) // compute sign flips due to anti commuting basis vectors
						if ((bitmap & (1 << bvIdx)) != 0) scale = -scale;
				}
			}

			public void multiply(float scalar)
			{
				scale *= scalar;
			}
		}

		private String inputString;
		private String sourceString;
		private int startIdx = 0;
		private int endIdx = 0;
		private int lineIdx = 0;
		private int currentLineStart = 0;
		private float[] coord = new float[32];

		// TOKEN IDs: (for internal use only)
		private const int T_BAD_IDENTIFIER = -100;
		private const int T_BAD_NUMBER = -10;
		private const int T_BAD_CHARACTER = -1;
		private const int T_END_OF_STRING = 0;
		private const int T_WEDGE = 1;
		private const int T_MUL = 2;
		private const int T_PLUS = 3;
		private const int T_MINUS = 4;
		private const int T_NUMBER = 10;
		private const int T_FIRST_BASIS_VECTOR = 100;
		private const int T_LAST_BASIS_VECTOR = 105;

		private ParseException GetException(String reason)
		{
			return new ParseException(reason + " at " + sourceString + ", line " + (lineIdx+1) + ", column " + (startIdx - currentLineStart +1));
		}

		private Parser(String input, String source) {
			inputString = input;
			sourceString = source;
			if ((sourceString == null) || (length(sourceString) == 0))
				sourceString = "string";
		}

		private void sum(BasisBlade bb) {
			int idx = c3ga.BasisElementIndexByBitmap[bb.getBitmap()];
			coord[idx] += bb.getScale() / (float)c3ga.BasisElementSignByIndex[idx];
		}

			private static bool isLetter(char x) {
    		return Char.IsLetter(x);
		}
		private static bool isDigit(char x) {
    		return Char.IsDigit(x);
		}
		private static bool isLetterOrDigit(char x) {
    		return Char.IsLetterOrDigit(x);
		}
		private String substring(int startIdx, int endIdx) {
    		return inputString.Substring(startIdx, endIdx - startIdx + 1);
		}
		private static int length(String str) {
    		return str.Length;
		}
		private static char charAt(String str, int idx) {
    		return str[idx];
		}    
		   

		private int getNextToken() {
			int l = length(inputString);
			
			// skip all whitespace and other empty stuff, keep track of line index
			while ((startIdx < l) && (charAt(inputString, startIdx) >= 0) && (charAt(inputString, startIdx) <= ' ')) {
				if (charAt(inputString, startIdx) == 0x0A) { // start of new line
					lineIdx++;
					currentLineStart = startIdx+1;
				}
				startIdx++;
			}

			// detect end of string
			if (startIdx == l) {
				startIdx = endIdx;
				return T_END_OF_STRING; // EOS
			}

			// operators
			if (charAt(inputString, startIdx) == '^') {endIdx = startIdx; return T_WEDGE;} // ^
			else if (charAt(inputString, startIdx) == '*') {endIdx = startIdx; return T_MUL;} // *
			else if (charAt(inputString, startIdx) == '+') {endIdx = startIdx; return T_PLUS;} // +
			else if (charAt(inputString, startIdx) == '-') {endIdx = startIdx; return T_MINUS;} // -

			else if (isDigit(charAt(inputString, startIdx)) || (charAt(inputString, startIdx) == '.')) { // parse number?
				endIdx = startIdx;

				// eat up all digits and at most one point
				bool pointFound = false;
				while ((endIdx < l) && (isDigit(charAt(inputString, endIdx)) || (charAt(inputString, endIdx) == '.'))) {
					endIdx++;
					if (charAt(inputString, endIdx) == '.') {
						endIdx++;
						pointFound = true;
						break;
					}
				}
	    		
				if (pointFound) { // if point found, eat up all digits
					while ((endIdx < l) && isDigit(charAt(inputString, endIdx))) {
						endIdx++;
					}
				}

				// see if there is a 'e' or 'E'
				if  ((charAt(inputString, endIdx) == 'e') || (charAt(inputString, endIdx) == 'E')) {
					endIdx++;
					// accept at most one +-
					if  ((charAt(inputString, endIdx) == '-') || (charAt(inputString, endIdx) == '+')) {
						endIdx++;
					}

					// if there is an 'e', there must be some digit
					if (!isDigit(charAt(inputString, endIdx))) return T_BAD_NUMBER; // bad number

					// eat up all digits
					while ((endIdx < l) && isDigit(charAt(inputString, endIdx))) {
						endIdx++;
					}
				}
				endIdx--; // end index is inclusive
				return T_NUMBER;
			}

			else if (isLetter(charAt(inputString, startIdx)) || (charAt(inputString, startIdx) == '_')) { // parse identifier?
				// find end of chain of numbers, letters and '_'
				endIdx = startIdx + 1;

				while ((endIdx < l) && (isLetterOrDigit(charAt(inputString, endIdx)) || (charAt(inputString, endIdx) == '_'))) endIdx++;
				endIdx--;  // end index is inclusive

				// see which basis vector it is
				String bvName = substring(startIdx, endIdx);
				for (int i = 0; i < 5; i++)
					if (c3ga.BasisVectorNames[i].Equals(bvName)) return T_FIRST_BASIS_VECTOR + i; // basis vector
				return T_BAD_IDENTIFIER; // bad identifier
			}

			else return T_BAD_CHARACTER;
		} // end of getNextToken()

		/// <summary>Parses a multivector string (e.g., as created by mv.ToString())
		/// Throws a ParseException when the input cannot be parsed.
		/// 
		/// </summary>
		/// <param name="str">The multivector string.
		/// </param>
		/// <param name="strSourceName">Description of where the string came from (may be null). This string is used in exception messages.
		/// </param>
		public static mv Parse(String str, String strSourceName)  
		{
			return new Parser(str, strSourceName).Parse();
		}
	    
		private mv Parse() {
			int token;
			bool firstLoop = true;

			// get the first token
			token = getNextToken();

			while (true) {
				// reset for next basis blade
				BasisBlade bb = new BasisBlade();
				bool beDone = false; // basis element done

				if (token == T_END_OF_STRING) break;

				int cnt = 0;
				while ((token == T_PLUS) || (token == T_MINUS)) { // accept all +- 
					cnt++;
					startIdx = endIdx+1;
					if (token == T_MINUS) bb.multiply(-1.0f); // -
					token = getNextToken();
				}
	    		
				// require at least one +- if this is not the first term:
				if ((!firstLoop) && (cnt == 0)) {
					throw GetException("Expected '+' or '-'");
				}

				if ((token == T_NUMBER) || 
					((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR))) // must be number or basis vector
				{ 
					if (token == T_NUMBER) {
						{ // multiply scale with value of number
							try {
								bb.multiply((float)Double.Parse(substring(startIdx, endIdx)));
							} catch (Exception ) {
								throw GetException("Cannot parse number"); 
							}
						}
						startIdx = endIdx+1;

						// * or ^ ?
						token = getNextToken();
						if ((token == T_WEDGE) || (token == T_MUL)) {
							startIdx = endIdx+1;

							// must find basis vector
							token = getNextToken();
						}
						else { // just a single scalar is OK
							startIdx = endIdx+1;
							beDone = true;
						}

					}

					if (!beDone) {
						if ((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR)) {
							int bvIdx = token - T_FIRST_BASIS_VECTOR;
							bb.wedgeBasisVector(bvIdx);
							startIdx = endIdx+1;
						}
						else {
							throw GetException("Expected basis vector");
						}
					}

					if (!beDone) {
						// accept ^ basis vector as many times as it takes
						while (true) {
							// ^
							token = getNextToken();
							if (token != T_WEDGE) break;
							startIdx = endIdx+1;

							// basis vector
							token = getNextToken();
							if ((token >= T_FIRST_BASIS_VECTOR) && (token <= T_LAST_BASIS_VECTOR)) {
								int bvIdx = token - T_FIRST_BASIS_VECTOR;
								bb.wedgeBasisVector(bvIdx);
								startIdx = endIdx+1;
							}
							else {
								throw GetException("Expected basis vector");
							}


						}

					}
				} // end of 'if number or bv'
				else if (token == T_BAD_CHARACTER) {
					throw GetException("Bad character");
				}
				else if (token == T_BAD_NUMBER) {
					throw GetException("Bad number");
				}
				else if (token == T_BAD_IDENTIFIER) {
					throw GetException("Bad identifier");
				}
				else {
					throw GetException("Unexpected token");
				}

				// add
				sum(bb);
	    		
				// remember that the first loop is done
				firstLoop = false;
			}

			
			mv result = new mv(GroupBitmap.GRADE_0 | GroupBitmap.GRADE_1 | GroupBitmap.GRADE_2 | GroupBitmap.GRADE_3 | GroupBitmap.GRADE_4 | GroupBitmap.GRADE_5 | 0,  coord);
			result.Compress(0.0f);
			return result;
		} // end of Parse()


	} // end of class Parser

}


