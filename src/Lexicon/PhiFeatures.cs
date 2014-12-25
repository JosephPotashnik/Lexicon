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

   public class FeatureDictionary
   {
       public Dictionary<string, int> Dictionary { get; set; }
       public int NumOfFeatures { get; set; }

       public FeatureDictionary(FeatureDefinitions defs)
       {
           NumOfFeatures = defs.FeatureNamesVector.FeatureNames.Count();
           Dictionary = new Dictionary<string, int>();
           foreach (Feature f in defs.Features)
           {
               foreach (string s in f.Values)
               {
                   for (int k = 0; k < NumOfFeatures; ++k)
                   {
                       if (f.Name == defs.FeatureNamesVector.FeatureNames[k])
                       {
                           Dictionary.Add(s, k);
                           break;
                       }
                   }
               }
           }
       }
   }
}
