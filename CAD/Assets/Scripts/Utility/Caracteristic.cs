namespace CAD.Utility {

    [System.Serializable]
    public class Caracteristic {

        public int ID;

        public string labels;

        public float mushape;

        public float muPos;

        public float mujoint;

        public float mustruct;

        public float localMeasure;

        public float partialMeasure;

        public float globalMeasure;
    }

    public class ModelsRetrievedData
    {

        public Clique clique;
        public Measure measure;
    }

    public class Clique
    {
    }

    [System.Serializable]
    public class Measure
    {

        public float shapeMeasure;

        public float jointMeasure;

        public float positionMeasure;

        public float localMeasure;

        public float partialMeasure;

        public float globalMeasure;
    }
}