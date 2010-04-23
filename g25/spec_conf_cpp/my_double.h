#ifndef _MY_DOUBLE_H_
#define _MY_DOUBLE_H_

/// Example of how to use a custom float type inside Gaigen 2.5
class myDouble {
public:
	inline myDouble() : value(0.0) {}
	inline myDouble(double d) : value(d) {}
	inline myDouble(const myDouble &D) : value(D.value) {}

	inline myDouble& operator=(const myDouble &D) {
		value = D.value;
		return *this;
	}

	inline double getValue() const {
		return value;
	}

	inline operator double() const {
		return value;
	}

	inline myDouble &operator+=(const myDouble &D) {
		value += D.getValue();
		return *this;
	}

	inline myDouble &operator-=(const myDouble &D) {
		value -= D.getValue();
		return *this;
	}

	inline myDouble &operator*=(const myDouble &D) {
		value *= D.getValue();
		return *this;
	}

	inline myDouble &operator/=(const myDouble &D) {
		value /= D.getValue();
		return *this;
	}

private:
	double value;
};

inline bool operator==(const myDouble &D1, const myDouble &D2) {
	return D1.getValue() == D2.getValue();
}


inline bool operator<(const myDouble &D1, const myDouble &D2) {
	return D1.getValue() < D2.getValue();
}

inline bool operator>(const myDouble &D1, const myDouble &D2) {
	return D1.getValue() > D2.getValue();
}

inline myDouble operator+(const myDouble &D1, const myDouble &D2) {
	return myDouble(D1.getValue() + D2.getValue());
}

inline myDouble operator-(const myDouble &D) {
	return myDouble(-D.getValue());
}

inline myDouble operator+(const myDouble &D) {
	return myDouble(D.getValue());
}

inline myDouble operator-(const myDouble &D1, const myDouble &D2) {
	return myDouble(D1.getValue() - D2.getValue());
}

inline myDouble operator*(const myDouble &D1, const myDouble &D2) {
	return myDouble(D1.getValue() * D2.getValue());
}

inline myDouble operator/(const myDouble &D1, const myDouble &D2) {
	return myDouble(D1.getValue() / D2.getValue());
}

inline myDouble fabs(const myDouble &D) {
	return myDouble(fabs(D.getValue()));
}

inline myDouble sqrt(const myDouble &D) {
	return myDouble(sqrt(D.getValue()));
}

inline myDouble tan(const myDouble &D) {
	return myDouble(tan(D.getValue()));
}

inline myDouble sin(const myDouble &D) {
	return myDouble(sin(D.getValue()));
}

inline myDouble cos(const myDouble &D) {
	return myDouble(cos(D.getValue()));
}

inline myDouble exp(const myDouble &D) {
	return myDouble(exp(D.getValue()));
}

inline myDouble sinh(const myDouble &D) {
	return myDouble(sinh(D.getValue()));
}

inline myDouble cosh(const myDouble &D) {
	return myDouble(cosh(D.getValue()));
}

inline myDouble tanh(const myDouble &D) {
	return myDouble(tanh(D.getValue()));
}

inline myDouble log(const myDouble &D) {
	return myDouble(log(D.getValue()));
}


#endif /* _MY_DOUBLE_H_ */
