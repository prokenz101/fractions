using System.Numerics;

namespace fractions {
    public class Program {
        static void Main(string[] args) {
            //* You can do whatever with the Fraction and MixedFraction class

            //* Examples:
            Fraction myFraction1 = new("1/2");
            Fraction myFraction2 = new("1/3");

            MixedFraction myMixedFraction1 = new(4, "1/2");
            MixedFraction myMixedFraction2 = new(1, "1/3");

            //* Print the fractions
            Console.WriteLine(myFraction1.ToString());
            Console.WriteLine(myFraction2.ToString());

            //* Add the fractions
            Fraction.Add(myFraction1, myFraction2);
            MixedFraction.Add(myMixedFraction1, myMixedFraction2);
            MixedFraction.Add(myMixedFraction1, myMixedFraction2, simplify: false); //* Adding without simplifying

            //* Converting between mixed and improper
            MixedFraction willbeConvertedToImproper = new(1, "1/2");
            willbeConvertedToImproper.ToImproperFraction();

            Fraction willBeConvertedToMixed = new("9/2");
            willBeConvertedToMixed.ToMixedFraction();

            //* Converting fractions to Decimal
            Fraction myFraction3 = new("1/2");
            myFraction3.ToDecimal();

            //* Convering fractions to percentage
            Fraction myFraction4 = new("1/2");
            myFraction4.ToPercentage();
        }
    }

    public class Fraction {
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }

        public Fraction(string fraction) {
            if (!(fraction.Contains('/'))) {
                throw new NoSlashException();
            }

            var parts = fraction.Split('/');
            Numerator = BigInteger.Parse(parts[0]);
            Denominator = BigInteger.Parse(parts[1]);

            if (Denominator == 0) {
                throw new DivideByZeroException();
            }
        }

        public override string ToString() { return $"{Numerator}/{Denominator}"; }

        public MixedFraction ToMixedFraction() {
            BigInteger Quotient = Numerator / Denominator;
            BigInteger Remainder = Numerator % Denominator;

            //* Q R/D
            return new MixedFraction(Quotient, $"{Remainder}/{Denominator}");
        }

        public double ToDecimal() { return (double)(Numerator / Denominator); }

        public double ToPercentage() { return (double)(Numerator / Denominator) * 100; }

        public bool IsProper() { return this.Denominator >= this.Numerator; }

        public bool IsImproper() { return this.Denominator < this.Numerator; }

        public static bool IsLike(Fraction fc1, Fraction fc2) { return fc1.Denominator == fc2.Denominator; }

        public static bool IsUnlike(Fraction fc1, Fraction fc2) { return fc1.Denominator != fc2.Denominator; }

        public static Fraction? Operation(Fraction fc1, Fraction fc2, string operation, bool simplify = true) {
            if (operation == "add") {
                return Fraction.Add(fc1, fc2, simplify);
            } else if (operation == "subtract") {
                return Fraction.Subtract(fc1, fc2, simplify);
            } else if (operation == "multiply") {
                return Fraction.Multiply(fc1, fc2, simplify);
            } else if (operation == "divide") {
                return Fraction.Divide(fc1, fc2, simplify);
            } else {
                return null;
            }
        }

        public static Fraction Add(Fraction fc1, Fraction fc2, bool simplify = true) {
            BigInteger lcm = LCM.FindLCM(new BigInteger[] { fc1.Denominator, fc2.Denominator });
            BigInteger numerator = fc1.Numerator * (lcm / fc1.Denominator) + fc2.Numerator * (lcm / fc2.Denominator);
            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Subtract(Fraction fc1, Fraction fc2, bool simplify = true) {
            BigInteger lcm = LCM.FindLCM(new BigInteger[] { fc1.Denominator, fc2.Denominator });
            BigInteger numerator = fc1.Numerator * (lcm / fc1.Denominator) - fc2.Numerator * (lcm / fc2.Denominator);
            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Multiply(Fraction fc1, Fraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                new Fraction($"{fc1.Numerator * fc2.Numerator}/{fc1.Denominator * fc2.Denominator}"), simplify
            );
        }

        public static Fraction Divide(Fraction fc1, Fraction fc2, bool simplify = true) {
            Fraction reciprocal = new Fraction($"{fc2.Denominator}/{fc2.Numerator}");
            return SimplifyIfRequired(Fraction.Multiply(fc1, reciprocal), simplify);
        }

        public Fraction ToSimplestForm() {
            //* get GCD of numerator and denominator
            BigInteger gcd = HCF.FindGCD(new BigInteger[] { Numerator, Denominator }, 2);

            Fraction fractionObtained = new(Numerator / gcd + "/" + Denominator / gcd);
            if (fractionObtained.Denominator < 0) {
                //* if denominator is negative, then convert to rational number simplest form
                fractionObtained.Numerator = 0 - fractionObtained.Numerator;
                fractionObtained.Denominator = -1 * fractionObtained.Denominator;
            }

            return fractionObtained;
        }

        public static Fraction SimplifyIfRequired(Fraction fc, bool simplify) {
            if (simplify) {
                return fc.ToSimplestForm();
            } else {
                return fc;
            }
        }

        public class NoSlashException : Exception {
            public NoSlashException() : base("Fraction must be in the form of a/b") { }
        }
    }

    public class MixedFraction {
        public BigInteger WholeNumber { get; set; }
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }

        public MixedFraction(BigInteger wholeNumber, string fraction) {
            if (!(fraction.Contains('/'))) { throw new Fraction.NoSlashException(); }
            string[] parts = fraction.Split("/");

            WholeNumber = wholeNumber;
            Numerator = BigInteger.Parse(parts[0]);
            Denominator = BigInteger.Parse(parts[1]);
        }

        public override string ToString() { return $"{WholeNumber} {Numerator}/{Denominator}"; }

        public Fraction ToImproperFraction() {
            return new Fraction($"{(WholeNumber * Denominator) + Numerator}/{Denominator}");
        }

        public double ToDecimal() {
            Fraction asImproper = this.ToImproperFraction();
            return (double)(asImproper.Numerator / asImproper.Denominator);
        }

        public double ToPercentage() {
            return (double)(this.ToDecimal() * 100);
        }

        public MixedFraction ToSimplestForm() {
            return new MixedFraction(
                WholeNumber, new Fraction($"{Numerator}/{Denominator}").ToSimplestForm().ToString()
            );
        }

        public static MixedFraction? Operation(
            MixedFraction mfc1, MixedFraction mfc2, string operation, bool simplify = true
        ) {
            if (operation == "add") {
                return MixedFraction.Add(mfc1, mfc2, simplify);
            } else if (operation == "subtract") {
                return MixedFraction.Subtract(mfc1, mfc2, simplify);
            } else if (operation == "multiply") {
                return MixedFraction.Multiply(mfc1, mfc2, simplify);
            } else if (operation == "divide") {
                return MixedFraction.Divide(mfc1, mfc2, simplify);
            } else {
                return null;
            }
        }

        public static MixedFraction Add(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Add(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Subtract(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Subtract(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Multiply(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Multiply(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Divide(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Divide(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction SimplifyIfRequired(MixedFraction mfc, bool simplify) {
            if (simplify) {
                return mfc.ToSimplestForm();
            } else {
                return mfc;
            }
        }
    }

    public class LCM {
        public static BigInteger FindLCM(BigInteger[] element_array) {
            BigInteger lcm_of_array_elements = 1;
            BigInteger divisor = 2;

            while (true) {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < element_array.Length; i++) {

                    if (element_array[i] == 0) {
                        return 0;
                    } else if (element_array[i] < 0) {
                        element_array[i] = element_array[i] * (-1);
                    }

                    if (element_array[i] == 1) {
                        counter++;
                    }

                    if (element_array[i] % divisor == 0) {
                        divisible = true;
                        element_array[i] = element_array[i] / divisor;
                    }
                }

                if (divisible) {
                    lcm_of_array_elements = lcm_of_array_elements * divisor;
                } else {
                    divisor++;
                }

                if (counter == element_array.Length) {
                    return lcm_of_array_elements;
                }
            }
        }
    }

    public class HCF {
        public static BigInteger FindHCF(BigInteger a, BigInteger b) {
            if (a == 0) { return b; }
            return FindHCF(b % a, a);
        }

        //* Function to find gcd of 
        //* array of numbers
        public static BigInteger FindGCD(BigInteger[] arr, BigInteger n) {
            BigInteger result = arr[0];
            for (BigInteger i = 1; i < n; i++) {
                result = FindHCF(arr[(int)i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
    }
}