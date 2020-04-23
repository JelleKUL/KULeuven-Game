
[System.Serializable]
public class DenkVraag
{
    
    public enum QuestionType { Geen, Waterpassing, Foutenpropagatie, Polygonatie, MapAngle }
    public enum Operator {Geen, AGelijkAanB, AGroterDanB, AKleinerDanB}

    public string vraagText;
    public QuestionType Thema;
    public bool isWaar;
    public string uitleg;

    //public string a;
    //public string b;
    //public Operator juisteVergelijking;





}
