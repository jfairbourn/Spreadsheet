// Skeleton written by Joe Zachary for C/S 3500, September 2012
// Version 1.1.  Clarified the specification of ToString and addeed
//               Equals, operator==, operator!=, and GetHashCode.
// Code implemented by James Fairbourn, 9/18/2012
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using floating-point
    /// syntax, variables that consist of one or more letters followed by one or more 
    /// digits, parentheses, and the four operator symbols +, -, *, and /.
    /// </summary>
    public class Formula
    {
        private String formula;             //initalizes a string to hold the formula.
        private IEnumerable<string> tokens; //initalizes a IEnumerable<string> for the tokens.
        public Func<string, bool> IsValid { get; protected set; }
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntacticaly invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// </summary>
        public Formula(String formula, Func<string, bool> IsValid)
        {
            this.IsValid = IsValid;
            this.formula = formula;             //sets formula to the String variable.
            tokens =GetTokens(formula);         //gets the tokens and stores them in tokens.
            try
            {
                this.Evaluate(s => 5.0);        //evaluates the current formula to make sure that it is viable.
            }
            catch (ArgumentException)           //throws an exception if it is not.
            {
                throw new FormulaFormatException("The expression sent in has a syntactical error");
            }

             
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  
        /// 
        /// Given a variable symbol as its parameter, lookup returns the
        /// variable's value (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            try
            {
                // two stacks created to hold values and operators.
                Stack<double> valueStack = new Stack<double>();
                Stack<string> operatorStack = new Stack<string>();
                //breaks apart the String holding the expression and put into an array.

                foreach (String s in tokens)        //each String is pulled from the array
                {
                    double num;
                    bool isNumber;
                    isNumber = double.TryParse(s, out num);  //trys parsing the String into an int
                    if (isNumber)                         //if it can be parsed then the stack is peeked
                    {                                     //to see what operator is at the top of the stack. 
                        if (!isEmpty(operatorStack))
                        {
                            String peekValue = operatorStack.Peek();
                            if (peekValue.Equals("*"))     //if the operator is * then an int is pulled from
                            {                              //the stack and multiplied with the current number
                                if (!isEmpty(valueStack))  //and pushed back into the value stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Mult(num, valueStack.Pop()));

                                }
                            }
                            else if (peekValue.Equals("/")) //if the operator is / then an int is pulled from
                            {                               //the stack and multiplied with the current number
                                if (!isEmpty(valueStack) && valueStack.Peek() != 0)   //and pushed back into the value stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Div(valueStack.Pop(), num));
                                }
                            }

                            else
                            {
                                valueStack.Push(num);       //if it is neither of these cases, then the number is pushed
                            }                               //into the value stack to be used later.
                        }
                        if (isEmpty(valueStack))
                        {
                            valueStack.Push(num);
                        }

                    }

                    else if (s.Equals("+") || s.Equals("-")) //if the current string is either a + or -, then the following is performed:
                    {
                        if (hasValues(valueStack))
                        {
                            String operPeek = operatorStack.Peek();   //peek to see the current operator
                            if (operPeek.Equals("+"))                 //if the current operator is +
                            {                                         //then two values are popped out of the stack
                                if (hasValues(valueStack))            //and added together and pushed back onto the value stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Add(valueStack.Pop(), valueStack.Pop()));
                                }
                            }
                            else if (operPeek.Equals("-"))            //if the current operator is -
                            {                                         //then two values are popped out of the stack
                                if (hasValues(valueStack))            //and subtracted from each other and pushed back onto the value stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Sub(valueStack.Pop(), valueStack.Pop()));
                                }
                            }
                        }
                        operatorStack.Push(s);                        //the current string that holds either a + or - is pushed onto operator stack.

                    }
                    else if (s.Equals("*") || s.Equals("/"))          //if the current string is a * or / operator, then it is pushed onto the operator stack.
                    {
                        operatorStack.Push(s);
                    }
                    else if (s.Equals("("))                           //if the current string is (, then it is pushed onto the operator stack.
                    {
                        operatorStack.Push(s);
                    }
                    else if (s.Equals(")"))                           //if the current string is ), then the following occurs:
                    {
                        String operPeek = operatorStack.Peek();       //look at the current operator
                        if (operPeek.Equals("+"))                     //if the current operator is +, the two values are poppped
                        {                                             //out of the stack and added and then pushed onto the value stack.
                            if (hasValues(valueStack))
                            {
                                operatorStack.Pop();
                                valueStack.Push(Add(valueStack.Pop(), valueStack.Pop()));
                                operatorStack.Pop();
                            }
                        }
                        else if (operPeek.Equals("-"))              //if the current operator is -, then two values are popped
                        {                                           //out of the value stack, subtracted from each other and then
                            if (hasValues(valueStack))              //pushed onto the value stack.
                            {
                                operatorStack.Pop();
                                double right = valueStack.Pop();
                                double left = valueStack.Pop();
                                valueStack.Push(left-right);
                                operatorStack.Pop();
                            }

                        }
                        else if (operPeek.Equals("*"))              //if the current operator is *, then two values are popped
                        {                                           //out of the value stack, mulptiplied and then pushed onto
                            if (!isEmpty(valueStack))               //onto the value stack.
                            {
                                operatorStack.Pop();
                                valueStack.Push(Mult(valueStack.Pop(), valueStack.Pop()));

                            }
                        }
                        else if (operPeek.Equals("/"))              //if the current operator is /, then two values are popped
                        {                                           //out of the value stack, divided and then pushed onto the 
                            if (!isEmpty(valueStack) && valueStack.Peek() != 0) //value stack.
                            {
                                operatorStack.Pop();
                                valueStack.Push(Div(valueStack.Pop(), valueStack.Pop()));
                            }
                        }
                        else if (operPeek.Equals("("))
                        {
                            operatorStack.Pop();
                        }
                    }
                    else if (!s.Equals(""))                         //if the current string is anything else, besides empty space,
                    {                                               //then it is treated like a variable.
                        if (!isEmpty(operatorStack))                //if the operator stack is not empty then:
                        {
                            String peekValue = operatorStack.Peek(); //peek at the current operator value.
                            if (peekValue.Equals("*"))               //if the current operator is *, then variableEvaluator is
                            {                                        //used for the variable value and another value is popped and 
                                if (!isEmpty(valueStack))            //multiplied together and pushed back into the stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Mult(lookup(s), valueStack.Pop()));

                                }
                            }
                            else if (peekValue.Equals("/"))         //if the current operator is /, then variableEvaluator is
                            {                                       //used for the variable value and another value is popped and
                                if (!isEmpty(valueStack) && valueStack.Peek() != 0)     //divided and pushed back into the stack.
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(Div(valueStack.Pop(), lookup(s)));
                                }
                            }
                            else
                            {
                                if (IsValid(s))
                                {
                                    valueStack.Push(lookup(s));    //if neither of the above conditions, then the value is found
                                }                                             //from the variableEvaluator then pushed onto the value stack.
                                else
                                {
                                    throw new FormulaFormatException("Invalid varible");
                                }
                            }
                        }
                        
                        if (isEmpty(valueStack))
                        {
                            if (!s.Equals("="))
                            {
                                valueStack.Push(lookup(s));
                            }
                        }
                    }
                }
                try
                {

                    if (isEmpty(operatorStack))     //if the operator stack is empty, then return the value at the top of the value stack.
                    {

                        return valueStack.Pop();

                    }
                    else                            //if the operator stack is not empty, then two values are popped and so is the operator
                    {                               //and either added, subtracted, multiplied or divided from each other.
                        try
                        {
                            if (operatorStack.Peek().Equals("("))
                            {
                                operatorStack.Pop();
                            }
                            String oper = operatorStack.Pop();
                            if (oper.Equals("+"))
                            {
                                return (Add(valueStack.Pop(), valueStack.Pop()));
                            }
                            else if (oper.Equals("-"))
                            {
                                double right = valueStack.Pop();
                                double left = valueStack.Pop();

                                return (left - right);
                            }
                            else if (oper.Equals("*"))
                            {
                                return (Mult(valueStack.Pop(), valueStack.Pop()));
                            }
                            else
                            {
                                return (Div(valueStack.Pop(), valueStack.Pop()));
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            throw new ArgumentException();      //if an exception is thrown, then the input was invalid.
                        }

                    }

                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException();
                }
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException();
            }
        }
        

        /// <summary>
        /// Enumerates all of the variables that occur in this formula.  No variable
        /// may appear more than once in the enumeration, even if it appears more than
        /// once in this Formula.
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> variables = new List<string>();        //Creates a new list.
            Regex reg = new Regex(@"([a-zA-Z]+)([1-9]\d*)");       //Gets everything that contains letters and excludes whitespace.

            foreach (String s in tokens)
            {
                Match match = reg.Match(s);
                if (match.Success)
                {
                    variables.Add(s);
                }
            }
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).
        /// </summary>
        public override string ToString()
        {
            return formula.Replace(" ", string.Empty);          //Removes whitespace from the string and returns.
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  All tokens are compared as strings except for numeric tokens,
        /// which are compared as doubles.
        /// 
        /// Here are some examples.  
        /// new Formula("x1+y2").Equals(new Formula("x1  +  y2")) is true
        /// new Formula("x1+y2").Equalas(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            List<string> variables = (List<string>)GetVariables();  //A list to hold variables.
           
            if(!(obj is Formula) || obj.Equals(null))               //if the object is not a Formula object or is null, return false.
            {
             return false;
            }   
                Formula form = (Formula)obj;                        //casts the object to a formula object.                       
                String form1 = form.ToString();                     //calls ToString on this and the object.
                String form2 = this.ToString();
            if (this.Evaluate(s => 0.0).Equals(form.Evaluate(s => 0.0)))        //evaluates each formula with a variable 0 to make sure they equate properly.
            {
                foreach (string s in variables)                             //scans throught the list to see if they contain the same variables.
                {
                    if (!form1.Contains(s))
                    {
                        return false;                                       //if they don't, returns false.
                    }
                }
                
                return true;                                                //if they do, returns true.
               
                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))                      //calls the Equals method on each formula to see if they equal each other.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (!f1.Equals(f2))                     //calls the Equals method on each formula to see if they are not equal to each other.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.
        /// </summary>
        public override int GetHashCode()
        {
            string form1 = this.ToString();             //Calls ToString on the formula and returns the HashCode.
                return form1.GetHashCode();
           
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of one or more
        /// letters followed by one or more digits, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z]+\d+";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
        /// <summary>
        /// Multiplies two ints together
        /// </summary>
        /// <param name="a">int</param>
        /// <param name="b">int</param>
        /// <returns>a*b</returns>
        private static double Mult(double a, double b)
        {
            return a * b;
        }
        /// <summary>
        /// Divides two ints. b cannot be equal to 0.
        /// </summary>
        /// <param name="a">int</param>
        /// <param name="b">int</param>
        /// <returns>a/b</returns>
        private static double Div(double a, double b)
        {
            if (b == 0)
            {
                throw new ArgumentException();
            }
            return a / b;


        }
        /// <summary>
        /// Adds two ints together.
        /// </summary>
        /// <param name="a">int</param>
        /// <param name="b">int</param>
        /// <returns>a+b</returns>
        private static double Add(double a, double b)
        {
            return a + b;
        }
        /// <summary>
        /// Subtracts two ints from each other.
        /// </summary>
        /// <param name="a">int</param>
        /// <param name="b">int</param>
        /// <returns>a-b</returns>
        private static double Sub(double a, double b)
        {
            return a - b;
        }
        /// <summary>
        /// Determines whether the current Stack of ints is empty.
        /// </summary>
        /// <param name="stack">Stack of ints</param>
        /// <returns>True if the stack is empty, false if it is not.</returns>
        private static bool isEmpty(Stack<double> stack)
        {
            if (stack.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///  Determines whether the current Stack of strings is empty.
        /// </summary>
        /// <param name="stack">Stack of string values</param>
        /// <returns>True if the stack is empty, false if it is not.</returns>
        private static bool isEmpty(Stack<String> stack)
        {
            if (stack.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether the Stack of ints has at least two values.
        /// </summary>
        /// <param name="stack">Stack of ints</param>
        /// <returns>True if the stack has at least two values, false if it does not.</returns>
        private static bool hasValues(Stack<double> stack)
        {
            if (stack.Count >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }

}

