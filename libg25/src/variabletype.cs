namespace G25
{

    /// <summary>
    /// Type of variable used by Gaigen 2 (scalars, multivectors, outermorphisms)
    /// </summary>
    public enum VARIABLE_TYPE
    {
        SCALAR = 1, // scalar value (float, double, etc)
        ENUM = 2, // enumerations

        MV = 10, // undefined multivector (returned by G25.MV type)
        GMV = 11, // general multivector
        SMV = 12, // specialized multivector

        OM = 20,   // undefined outermorphism (returned by G25.OM type)
        GOM = 21, // general outermorphism
        SOM = 22 // specialized outermorphism

    }

    /// <summary>
    /// Alls variable types must implement this interface.
    /// </summary>
    public interface VariableType
    {
        /// <returns>the metatype of the variable (scalar, multivector, etc).</returns>
        VARIABLE_TYPE GetVariableType();

        /// <returns>the name of the variable type ("float", "rotor", "mv", etc).</returns>
        string GetName();
    }

    /// <summary>
    /// A type for enumerations.
    /// 
    /// Names are never mangled.
    /// </summary>
    public class EnumType : VariableType
    {
        public EnumType(string enumName)
        {
            m_name = enumName;
        }

        public VARIABLE_TYPE GetVariableType()
        {
            return VARIABLE_TYPE.ENUM;
        }

        /// <returns>the name of the variable type ("float", "rotor", "mv", etc).</returns>
        public string GetName()
        {
            return m_name;
        }

        protected string m_name;
    }




} // end of namespace G25
