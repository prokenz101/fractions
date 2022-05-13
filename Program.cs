﻿namespace fractions {
    public class Program {
        static void Main(string[] args) {
            //* do X with the Fraction and MixedFraction classes

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
        }
    }

    public class Fraction {
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public Fraction(string fraction) {
            if (!(fraction.Contains('/'))) {
                throw new NoSlashException();
            }

            var parts = fraction.Split('/');
            Numerator = int.Parse(parts[0]);
            Denominator = int.Parse(parts[1]);

            if (Denominator == 0) {
                throw new DivideByZeroException();
            }
        }

        public override string ToString() {
            return $"{Numerator}/{Denominator}";
        }

        public MixedFraction ToMixedFraction() {
            (int Quotient, int Remainder) = Math.DivRem(Numerator, Denominator);

            //* Q R/D
            return new MixedFraction(Quotient, $"{Remainder}/{Denominator}");
        }

        public double ToDecimal() {
            return (double)Numerator / Denominator;
        }

        public static Fraction Add(Fraction fc1, Fraction fc2, bool simplify = true) {
            int lcm = LCM.FindLCM(new int[] { fc1.Denominator, fc2.Denominator });
            int numerator = fc1.Numerator * (lcm / fc1.Denominator) + fc2.Numerator * (lcm / fc2.Denominator);

            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Subtract(Fraction fc1, Fraction fc2, bool simplify = true) {
            int lcm = LCM.FindLCM(new int[] { fc1.Denominator, fc2.Denominator });
            int numerator = fc1.Numerator * (lcm / fc1.Denominator) - fc2.Numerator * (lcm / fc2.Denominator);
            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Multiply(Fraction fc1, Fraction fc2, bool simplify = true) {
            return SimplifyIfRequired(new Fraction($"{fc1.Numerator * fc2.Numerator}/{fc1.Denominator * fc2.Denominator}"), simplify);
        }

        public static Fraction Divide(Fraction fc1, Fraction fc2, bool simplify = true) {
            Fraction reciprocal = new Fraction($"{fc2.Denominator}/{fc2.Numerator}");

            return SimplifyIfRequired(Fraction.Multiply(fc1, reciprocal), simplify);
        }

        public Fraction ToSimplestForm() {
            //* get GCD of numerator and denominator
            int gcd = HCF.FindGCD(new int[] { Numerator, Denominator }, 2);

            //* return new fraction
            return new Fraction(Numerator / gcd + "/" + Denominator / gcd);
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
        public int WholeNumber { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public MixedFraction(int wholeNumber, string fraction) {
            if (!(fraction.Contains('/'))) { throw new Fraction.NoSlashException(); }
            string[] parts = fraction.Split("/");

            WholeNumber = wholeNumber;
            Numerator = int.Parse(parts[0]);
            Denominator = int.Parse(parts[1]);
        }

        public override string ToString() {
            return $"{WholeNumber} {Numerator}/{Denominator}";
        }

        public Fraction ToImproperFraction() {
            return new Fraction($"{(WholeNumber * Denominator) + Numerator}/{Denominator}");
        }

        public MixedFraction ToSimplestForm() {
            return new MixedFraction(
                WholeNumber, new Fraction($"{Numerator}/{Denominator}").ToSimplestForm().ToString()
            );
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
        public static int FindLCM(int[] element_array) {
            int lcm_of_array_elements = 1;
            int divisor = 2;

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
        public static int FindHCF(int a, int b) {
            if (a == 0)
                return b;
            return FindHCF(b % a, a);
        }

        //* Function to find gcd of 
        //* array of numbers
        public static int FindGCD(int[] arr, int n) {
            int result = arr[0];
            for (int i = 1; i < n; i++) {
                result = FindHCF(arr[i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
    }
}