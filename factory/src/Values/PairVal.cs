public class PairVal(FactVal item1, FactVal item2) : FactVal{
    public readonly FactVal item1 = item1;
    public readonly FactVal item2 = item2;

    public void Deconstruct(out FactVal item1, out FactVal item2){
      item1 = this.item1;
      item2 = this.item2;
    }
   }
