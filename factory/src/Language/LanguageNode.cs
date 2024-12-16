using static GenParse.Util.Formatting;

namespace Factory;

public interface LanguageNode : ITree<LanguageNode>
{
  FactoryType CalculateType(TypeContext context);
}
