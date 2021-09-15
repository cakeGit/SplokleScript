
namespace sks
{
    public class sksvar {
        string varName;
        dynamic data;

        public string getVarName () {
            return varName;
        }

        public object getDataObj () {
            return data;
        }

        public sksvar (string vName, object dat) {
            varName = vName;
            data = dat;
        }

        public class numValue {
            public numValue(float val) {
                value = val;
            }

            public string toString () {
                return value.ToString() + " (Type : Sks Number)";
            }
            public float getValue() {
                return value;
            }
            float value;
        }
    }
}