public enum ArrayType
{
  inputArray,
  outputArray,
  altArray,
}

public class AnnotatedArrayValue
{
  public readonly ArrayType arrayType;
  public readonly object[] array;

  public AnnotatedArrayValue(ArrayType arrayType, object[] array)
  {
    this.arrayType = arrayType;
    this.array = array;
  }
}
