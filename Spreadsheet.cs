//implemented by James Fairbourn 9/26/2012
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SS;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Xml;


namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet 
    {
        private DependencyGraph dg;  //dependency graph to check on dependencies
        private HashSet<Cell> cells;    //HashSet to hold all cells.
        private String filename;
        private bool change;
        
        public Spreadsheet():base(s=>true, s => s, "default")
        {
            this.IsValid = IsValid;
            this.Normalize = Normalize;
            dg = new DependencyGraph();
            cells = new HashSet<Cell>();
            change = false;
          
        }
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version):base(isValid, normalize, version)
        {
            this.IsValid = isValid;
            this.Normalize = normalize;
            dg = new DependencyGraph();
            cells = new HashSet<Cell>();
            change = false;
        }
        public Spreadsheet(String filename, Func<string, bool> isValid, Func<string, string> normalize, string version):base(isValid, normalize, version)
        {
            this.IsValid = isValid;
            this.Normalize = normalize;
            dg = new DependencyGraph();
            cells = new HashSet<Cell>();
            this.filename = filename;
            change = false;
            if (GetSavedVersion(filename) != version)
            {
                throw new SpreadsheetReadWriteException("Conflicting versions of file.");
            }
            try
            {
                ReadandSetFile(filename);
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Could not read file");
            }
        }
        /// <summary>
        /// Used to read a file and then set all Cells.
        /// </summary>
        /// <param name="filename"></param>
        private void ReadandSetFile(String filename)
        {
            string cellName = "";                               //temp string.
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":                    
                                break;

                            case "cell":
                                break;

                            case "name":                        //collect the name.
                                reader.Read();
                                cellName = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                SetContentsOfCell(cellName, reader.Value);      //add the content and name into a new cell.
                                break;
                        }
                    }
                }

            }
        }
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> names = new List<string>();        //create a new list to hold the strings.
            foreach (Cell cell in cells)                    //for each cell in the hashset
            {
                names.Add(cell.getName());                  //get the name of the cell and add it to the list.
            }
            return names;                                   //return the list.
        }

        public override object GetCellContents(string name)
        {
            if(name==null||(!IsValid(name)))               //if the name is null or it is an invalid name, then return InvalidNameExeption()
            {
                throw new InvalidNameException();
            }
            foreach (Cell cell in cells)                    //for each cell in the hashset.
            {
                if (cell.getName().Equals(name))            //if the cell name equals the parameter name, return the content of that cell.
                {
                    return cell.getContents();
                }
            }
            throw new InvalidNameException();
        }

        protected override ISet<string> SetCellContents(string name, double number)
        {
            if(number==null)                                //if the number is null, throw an ArguementNullException. 
            {
                throw new ArgumentNullException();
            }
            if(name==null||(!IsValid(name)))               // if the name is null or invald, throw and InvalidNameException.
            {
                throw new InvalidNameException();
            }
            String cellName = Normalize(name);
            HashSet<string> dependents = new HashSet<string>();     //A SortedSet to hold the dependents.
            foreach (Cell cell in cells)                                // for each cell in the HashSet
            {
                if (cell.getName().Equals(cellName))                        //if the cell name equals the parameter name.
                {
                    foreach (String s in dg.GetDependees(cellName))        //for each dependents name.
                    {
                        dg.RemoveDependency(s, cellName);               //remove the dependency.
                    }
                    cell.setNumber(number);                     //set the number of the cell.
                    
                    foreach (String s in GetCellsToRecalculate(cellName))   //for each string in getcellstorecalculate.
                    {                                                   //add the string to dependents.
                        dependents.Add(s);

                    }
                    return (dependents);                                   //return the sorted set.
                }
                
            }

            cells.Add(new Cell(cellName, number));                              //otherwise, create a new cell and add it to the hashset.

            foreach (String s in GetCellsToRecalculate(cellName))           //for each string in getcellstorecalculate, add the string to the sortedset.
            {
                dependents.Add(s);

            }
            return dependents;                                          // return the sorted set.
        }

        protected override ISet<string> SetCellContents(string name, string text)
        {
            if(text==null)                                              //if the text is null, throw an ArgumentNullException.
            {
                throw new ArgumentNullException();
            }
            if(name==null|| (!IsValid(name)))                          //if the name is null or invalid, throw an InvalidNameException
            {
                throw new InvalidNameException();
            }
            String cellName = Normalize(name);
            HashSet<string> dependents = new HashSet<string>();     //a new sorted set to hold the dependents.
            foreach (Cell cell in cells)                                //for each cell in the hashset.
            {
                if (cell.getName().Equals(cellName))                        //if cell name equals the name parameter.
                {
                    foreach (String s in dg.GetDependees(cellName))        //for each string in getDependents.
                    {               
                        dg.RemoveDependency(s, cellName);                   //remove the dependency.
                    }
                    cell.setText(text);                                 //set the cell text to the parameter.
                    foreach (String s in GetCellsToRecalculate(cellName))   //for each string in getcellstorecalculate.
                    {
                        dependents.Add(s);                              //add the strings to the sorted set.

                    }
                    return (dependents);                                //return the sorted set.
                }
               
            }

            cells.Add(new Cell(cellName, text));                            //otherwise, create a new cell with the parameters.
            
            foreach (String s in GetCellsToRecalculate(cellName))           //for each string in getcellstorecalculate.
            {
                dependents.Add(s);                                      //add the string to the sorted set.

            }
            return dependents;                                          //return the sorted set.
           
        }

        protected override ISet<string> SetCellContents(string name, SpreadsheetUtilities.Formula formula)
        {
            if (name==null)                                             //if the name is null, throw ArgumentNullException.
            {
                throw new ArgumentNullException();
            }
            if (!IsValid(name))                                        //if name is invalid, throw InvalidNameException.
            {
                throw new InvalidNameException();
            }
            String cellName = Normalize(name);
            HashSet<string> dependents = new HashSet<string>();     //A SortedSet to hold the dependents.
            foreach (Cell cell in cells)                                //for each cell in the hashset.
            {
                if (cell.getName().Equals(cellName))                        //if the cell name equals the parameter name.
                {
                    cell.setFormula(formula);                           //set the cell formula to the parameter.
                    dg.ReplaceDependees(cellName, formula.GetVariables());  //replace the dependees.
                    
                        foreach (String s in GetCellsToRecalculate(cellName))   //for each string in getcellstorecalculate.
                        {
                            dependents.Add(s);                          //add string to the sorted set.
                        }
                   
                   

                    return (dependents);                                //return the sorted set.


                }
                
            }
            Formula cellForm = new Formula("="+formula.ToString(), s=>true);
            cells.Add(new Cell(cellName, cellForm));                         //otherwise, create a new cell using the parameters.
            foreach (string s in formula.GetVariables())                //for each string in getvariables.
            {
                dg.AddDependency(s, cellName);                              //add a new dependency.
            }
            foreach (String s in GetCellsToRecalculate(cellName))           //for each string in getcellstorecalculate.
            {
                dependents.Add(s);                                      //add the string to the sorted set.

            }
            return (dependents);                                        //return the sorted set.
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(name==null)                                              //if the name is null, throw ArgumentNullException
            {
                throw new ArgumentNullException();
            }
            if(!IsValid(name))                                         //if the name is invalid, throw InvalidNameException.
            {
                throw new InvalidNameException();
            }
           String cellName = Normalize(name);
           return dg.GetDependents(cellName);                               //return the dependents of name.
        }
        /// <summary>
        /// Tests wheter the current name is valid.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NameTest(String name)
        {
            
            Regex reg = new Regex(@"([a-zA-Z]+)([1-9]\d*)");            //the pattern to test.
            Match match = reg.Match(name);                              //see if name is a match.
            if(match.Success)                                           //if the name is a match, return true.
            {
                return true;
            }
            else
            {                                                           //else return false.
                return false;
            }
            
        }

        public override bool Changed
        {
            get
            {
               return true;
            }
            protected set
            {
                change = value;
            }
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    String s = reader.GetAttribute("version");      //get the attributes from the current version.
                                    return s;
                            }
                        }

                    }
                    return "";
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Could not open file.");
            }

        }

        public override void Save(string filename)
        {
          
            using(XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartDocument();                            //sets up the correct format for the output xml file.
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", this.Version);

                foreach(Cell cell in cells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name",cell.getName());
                    writer.WriteElementString("contents", cell.getContents().ToString());
                    writer.WriteEndElement();

                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }

        public override object GetCellValue(string name)
        {
            if (name == null || (!IsValid(name)))               //if name is null or not valid, throws an exception.
            {
                throw new InvalidNameException();
            }
            foreach (Cell cell in cells)                        //find the current type of the cell and returns the content.
            {
                if (cell.getName().Equals(name))
                {
                    Object content = cell.getContents();
                    if(content.GetType() == typeof(string))
                    {
                        return content;
                    }
                    else if (content.GetType() == typeof(Double))
                    {
                        return content;
                    }
                    else
                    {

                        Formula form = (Formula)content;        //if the cell is a Formula, evaluates it and then returns.
                        return(form.Evaluate(lookup));

                    }
                }
            }
            
            throw new InvalidNameException();
        }
            private double lookup(String name)
            {
                string cellName = Normalize(name);
                double value =(double)GetCellValue(name.ToUpper());
                return value;
            }
        

        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            change = true;
            if (content == null)                                            //if content is null, throw an exception.
            {
                throw new InvalidNameException();
            }
            if((name==null)||(!IsValid(name)))                              //if name is null or not valid throw and exception.
            {
                throw new InvalidNameException();
            }
            double dContent;
            bool doubleParse = Double.TryParse(content, out dContent);      //try and parse out a double.
            ISet<string> set = new HashSet<string>();
            if (doubleParse == true)
            {
               set =  SetCellContents(name, dContent);                      //if parse successful setcellcontents using name and the double.
              
            }
            else if (content.StartsWith("="))
            {
                String sForm = content.Substring(1, content.Length - 1); //if the content starts with = then it is a formula and content is set as such.
                sForm = sForm.ToUpper();                                                                
                Formula form = new Formula(sForm, IsValid);
                set = SetCellContents(name, form);

            }
            else
            {
                set = SetCellContents(name, content);                       //else it is a string and is set as such.
            }

            return set;
        }
    }
}

