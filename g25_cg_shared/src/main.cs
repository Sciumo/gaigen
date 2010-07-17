
namespace G25.CG.Shared
{
    /// <summary>
    /// Common functionality for C & C++ main code generation entry point
    /// </summary>
    public class Main
    {
        public const string GETTER_PREFIX = "get_";
        public const string SETTER_PREFIX = "set_";

        public const string MERSENNE_TWISTER = "MERSENNE_TWISTER";
        public const string NEED_TIME = "NEED_TIME";

        /// <summary>Multivector interface suffix (used for C# and Java)</summary>
        public const string IF_SUFFIX = "_if";

        /// <summary>
        /// Name of namespace for runtime computation of geometric product.
        /// </summary>
        public const string RUNTIME_NAMESPACE = "runtime";

        /// <summary>
        /// How many coordinates to set to zero / copy explicitly
        /// </summary>
        public const int MAX_EXPLICIT_ZERO = 16;


        public CoGsharp.CoG InitCog(Specification S)
        {
            // get cog, add references, load templates
            CoGsharp.CoG cog = new CoGsharp.CoG();
            cog.AddReference(S.GetType().Assembly.Location); // add reference for libg25
            cog.AddReference(RefGA.BasisBlade.ZERO.GetType().Assembly.Location); // add reference for RefGA
            cog.AddReference(this.GetType().Assembly.Location); // add reference for this assembly
            cog.AddReference((new G25.CG.Shared.Util()).GetType().Assembly.Location); // add reference for g25_cg_shared

            LoadTemplates(S, cog);
            return cog;
        }


        /// <summary>
        /// Should load all templates for the language into 'cog'.
        /// 
        /// Default implementation does nothing.
        /// 
        /// </summary>
        /// <param name="cog">Templates are loaded into this variable.</param>
        /// <param name="S">Specification. Used to know whether testing code will be generated.</param>
        public virtual void LoadTemplates(Specification S, CoGsharp.CoG cog)
        {
        }
    }
} // end of namespace G25.CG.Shared
