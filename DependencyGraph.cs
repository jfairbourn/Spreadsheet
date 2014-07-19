// Skeleton implementation written by Joe Zachary for CS 3500, September 2012.
// Version 1.0
// Implementation written by James Fairbourn for CS 3500, 9/11/2012
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
       /// <summary>
       /// The data structure that holds the ordered pair of strings.
       /// The HashSet contains the KeyValuePairs.  The KeyValuePair holds
       /// the two string values of the string ordered pairs (s,t).
       /// </summary>
    private HashSet<KeyValuePair<string, string>> dg;
       
        /// <summary>
        /// A temporary list that holds the same value as dg.
        /// </summary>
    private HashSet<KeyValuePair<string, string>> tempList;  
        
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            
            dg = new HashSet<KeyValuePair<string, string>>();                   //initiates each data structure.
            tempList = new HashSet<KeyValuePair<string, string>>();
          
        }

        
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {int count = 0;                                         //sets a count variable
            foreach (KeyValuePair<string, string> pair in dg)       //loops each pair in dg and increments count by 1.
            {
                count++;
            }
            return count;                                           //returns the count, which is the number of ordered pairs.
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            
            get {
                int count = 0;                                  //sets a count variable.
                foreach (string str in GetDependees(s))         //calls GetDependees with string s as the parameter and loops through the IENumberable<string>
                {                                               //that is returned.  For each string, the count is incremented by 1.
                    count++;
                }
                return count;                                   //return count.
               
                }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            int count = 0;                                    //sets the count variable.
            foreach (string str in GetDependents(s))          //calls GetDependents with string s as the parameter and loops through the IENumberable<string> 
            {                                                 //that is returned.  For each string, the count is incremented by 1.
                count++;
            }
            if (count > 0)                                    //if count is greater than 1, then the string s has dependents and true is returned.
            {
                return true;
            }
            else
            {                                                //if count is not greater than 1, then s does not have dependents and false is returned.               
                return false;
            }
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            int count = 0;                                  //sets count variable.
            foreach (string str in GetDependees(s))         //calls GetDependendees with string s as the parameter and loops through the IENumberable<string>
            {                                               //that is returned.  For each string, the count is incremented by 1.
                count++;                                    
            }
            if (count > 0)                                  //if count is greater than 1, then the string s has dependees and true is returned.
            {
                return true;
            }
            else
            {                                               //if count is not greater than 1, then s does not have dependents and false is returned.
                return false;
            }
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            List<string> dependents = new List<string>();   //Creates a list to hold the dependents.
            foreach(KeyValuePair<string, string> str in dg) //for each pair in dg.
            {
                if(str.Key.Equals(s))                       //if the key of the KeyValuePair equals the parameter s, then adds the value to the list.
                {
                    dependents.Add(str.Value);
                }
            }
            return dependents;                              //return dependents list.
        }


        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)   
        {
            List<string> dependees = new List<string>();        //Creates a list to hold the dependees.
            foreach (KeyValuePair<string, string> str in dg)    //for each pair in dg.
            {
                if (str.Value.Equals(s))                        //if the value of the KeyValuePair equals the parameter s, then adds the key to the list.
                {
                    dependees.Add(str.Key);
                }
            }

            return dependees;                                   //return the dependees list.
        }


        /// <summary>
        /// Adds the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void AddDependency(string s, string t)
        {
            KeyValuePair<string, string> pair = new KeyValuePair<string, string>(s, t);
            dg.Add(pair);
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            KeyValuePair<string, string> comp = new KeyValuePair<string,string>(s, t);      //creates a KeyValuePair of (s,t) and compares it to the list of dg.
            if (dg.Contains(comp))
            {
                dg.Remove(comp);                                                            //if the pair is in their, then it is removed from the list.
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            tempList.Clear();                                                               //clears the temporary list.
            foreach (KeyValuePair<string, string> pair in dg)                               //loops through each pair in dg.
            {
                
                if (pair.Key.Equals(s))                                                     //if the key of the pair equals s, then the pair is added to the temp list.
                {
                    tempList.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
                    
                }
            }
            foreach (KeyValuePair<string, string> pairs in tempList)                        //loops through the temp list, and removes the dependency (s, r).
            {
                RemoveDependency(pairs.Key, pairs.Value);
            }
            foreach (string str in newDependents)                                           //New dependency added in the form (s, t).
            {
                AddDependency(s, str);
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {

            tempList.Clear();                                                       //clears the temporary list.
            
            foreach (KeyValuePair<string, string> pair in dg)                       //loops through each pair in dg.
            {
                if (pair.Value.Equals(s))                                           //if the value of each pair equals s, then the pair is added to the temp list.
                {
                    tempList.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
                    
                }
            }
            foreach (KeyValuePair<string, string> pairs in tempList)                //loops through the temp list and removes the dependency in dg in the form (r,s).
            {
                RemoveDependency(pairs.Key, pairs.Value);
            }
            foreach (string str in newDependees)                                    //creates a new depency in the form of (t,s).
            {
                AddDependency(str, s);
            }
        }

    }

}


