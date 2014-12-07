using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon
{

  public class Feature
  {
      //name of the feature, e.g. Person, Number, Gender..
      public string Name { get; set; }
      //possible values of the feature, e.g. Singular, Plural, Masculine, Feminine..
      public string[] Values { get; set; }
  }

   //vector defining the names of the the relevant (morphological) features for a lexeme. e.g. [Person, Number, Gender, Tense..]
   public class FeatureVecDefinition
   {
       public string[] FeatureNames { get; set; }
   }
    
   
   public class FeatureDefinitions
   {
       //the vector of features names.
       public FeatureVecDefinition FeatureNamesVector { get; set; }
       public Feature[] Features { get; set; }

   }
    
   //values of a given feature vector. e.g. [person1, singular, feminine, past ... ]
   /*class FeatureVec
   {
       public string[] Features { get; set; }
   }*/
}
