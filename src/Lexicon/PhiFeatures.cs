using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon
{
    enum NumberEnum
    {
        Singular,
        Plural,
        Anything
    };

    enum PersonEnum
    {
        Person1,
        Person2,
        Person3,
        Anything
    }

    enum GenderEnum
    {
        Masculine,
        Feminine,
        Anything
    };

    enum MorphologicalFeatureVectorEnum
    {
        Gender,
        Number,
        Person,
    }

    class PhiFearture
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public PhiFearture(string _name)
        {
            name = _name;

        }
    }

    class PersonFeature : PhiFearture
    {
        PersonEnum val;
        public PersonFeature(PersonEnum _val) : base("Person") { val = _val; }
    }
    class GenderFeature : PhiFearture
    {
        GenderEnum val;
        public GenderFeature(GenderEnum _val) : base("Gender") { val = _val; }
    }

    class NumberFeature : PhiFearture
    {
        NumberEnum val;
        public NumberFeature(NumberEnum _val) : base("Number") { val = _val; }
    }

    class FeaturesVector
    {

        PhiFearture[] vec = new PhiFearture[3];

        public FeaturesVector(PersonEnum p, GenderEnum e, NumberEnum n)
        {
            vec[(int)MorphologicalFeatureVectorEnum.Person] = new PersonFeature(p);
            vec[(int)MorphologicalFeatureVectorEnum.Gender] = new GenderFeature(e);
            vec[(int)MorphologicalFeatureVectorEnum.Number] = new NumberFeature(n);
        }
    }
}
