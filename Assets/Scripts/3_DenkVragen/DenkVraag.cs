
//*************** serialized class where all the info about a question can be stored ***************//

[System.Serializable]
public class DenkVraag
{
    // the enumerators
    public enum QuestionType { Geen, Waterpassing, Foutenpropagatie, Polygonatie, MapAngle }
    public enum Operator {Geen, AGelijkAanB, AGroterDanB, AKleinerDanB}

    // the variables
    public string vraagText;
    public QuestionType Thema;
    public bool isWaar;
    public string uitleg;



    //public string a;
    //public string b;
    //public Operator juisteVergelijking;
	//jens was here

}
