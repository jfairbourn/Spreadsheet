//Implemented by James Fairoburn 9/26/2012
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// This class constructs the specifications of a Cell
    /// </summary>
    public class Cell
    {
        private String name;            //private member variables.
        private String text;
        private double number;
        private Formula formula;
        private bool isNumber = false;  //booleans for each type of variable
        private bool isText = false;
        private bool isFormula = false;

        /// <summary>
        /// Creates a Cell that has a string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public Cell(String name, String text)
        {
            this.name = name;
            this.text = text;
            isText = true;
        }
        /// <summary>
        /// Creates a Cell that has a double.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        public Cell(String name, double number)
        {
            this.name = name;
            this.number = number;
            isNumber = true;
        }
        /// <summary>
        /// Creates a cell that has a formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public Cell(String name, Formula formula)
        {
            this.name = name;
            this.formula = formula;
            this.isFormula = true;
          
        }
        /// <summary>
        /// Gets the name of the current Cell.
        /// </summary>
        /// <returns></returns>
        public String getName()
        {
            return this.name;
        }
        /// <summary>
        /// Sets the name of the current Cell.
        /// </summary>
        /// <param name="name"></param>
        public void setName(String name)
        {
            this.name = name;
        }
        /// <summary>
        /// Gets the number of the current Cell.
        /// </summary>
        /// <returns></returns>
        public double getNumber()
        {
            return this.number;
        }
        /// <summary>
        /// Sets the number of the current Cell.
        /// </summary>
        /// <param name="number"></param>
        public void setNumber(double number)
        {
            this.number = number;
            this.isText = false;
            this.isFormula = false;
            this.isNumber = true;
        }
        /// <summary>
        /// Gets the Formula of the current Cell.
        /// </summary>
        /// <returns></returns>
        public Formula getFormula()
        {
            return this.formula;
        }
        /// <summary>
        /// Sets the Formula of the current Cell.
        /// </summary>
        /// <param name="formula"></param>
        public void setFormula(Formula formula)
        {
            this.formula = formula;
            this.isText = false;
            this.isFormula = true;
            this.isNumber = false;

        }
        /// <summary>
        /// Gets the text of the current Cell.
        /// </summary>
        /// <returns></returns>
        public String getText()
        {
            return this.text;

        }
        /// <summary>
        /// Sets the text of the current Cell.
        /// </summary>
        /// <param name="text"></param>
        public void setText(String text)
        {
            this.text = text;
            this.isText = true;
            this.isFormula = false;
            this.isNumber = false;
        }
        /// <summary>
        /// Depending on the type, returns the content of the current Cell.
        /// </summary>
        /// <returns></returns>
        public Object getContents()
        {
            if (isText)             //if the cell holds a string, return text.
            {
                return text;
            }
            else if (isNumber)      //if the cell holds a number, return a number.
            {
                return number;
            }
            else
            {                       //else, the Cell holds a Formula, which is returned.
                return formula;
            }
        }
        public void setIsNumber(bool isNumber)
        {
            this.isNumber = isNumber;
        }
        public void setIsText(bool isText)
        {
            this.isText = isText;
        }
        public void setIsFormula(bool isFormula)
        {
            this.isFormula = isFormula;
        }
      
    }
}
