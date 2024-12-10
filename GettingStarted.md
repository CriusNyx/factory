# Simple recipes

## Example 1

The following program prints the resources needed to produce 1 Reinforced Iron Plate.
You can see the code and output below.

``` factory factory --stream
print ReinforcedIronPlate()
```

```#output factory --stream
ReinforcedIronPlate  
                     
1 ReinforcedIronPlate
|-6 IronPlate        
| |-9 IronIngot      
| | |-9 IronOre      
|-12 Screw           
| |-3 IronRod        
| | |-3 IronIngot    
| | | |-3 IronOre
```

## Example 2

We can pass a number into the parentheses to change the amount being produced.

```factory factory --stream
print ReinforcedIronPlate(5)
```

```#output factory --stream
ReinforcedIronPlate  
                     
5 ReinforcedIronPlate
|-30 IronPlate       
| |-45 IronIngot     
| | |-45 IronOre     
|-60 Screw           
| |-15 IronRod       
| | |-15 IronIngot   
| | | |-15 IronOre
```

## Example 3

We can also specify that we want to start at ingots instead of ore by using the `in` parameter.

```factory factory --stream
print ReinforcedIronPlate(5, in IronIngot)
```

```#output factory --stream
ReinforcedIronPlate  
                     
5 ReinforcedIronPlate
|-30 IronPlate       
| |-45 IronIngot     
|-60 Screw           
| |-15 IronRod       
| | |-15 IronIngot
```

## Example 4

The `tally` parameter can be used to list the amounts of a particular material needed.

```factory factory --stream
print ReinforcedIronPlate(5, in IronIngot, tally IronIngot)
```

```#output factory --stream
ReinforcedIronPlate  
                     
5 ReinforcedIronPlate
|-30 IronPlate       
| |-45 IronIngot     
|-60 Screw           
| |-15 IronRod       
| | |-15 IronIngot  

IronIngot 60
```

Or with the addition of the `inline` keyword the tallys can be displayed in a list.

```factory factory --stream
print ReinforcedIronPlate(5, in IronIngot, tally inline IronIngot)
```

```#output factory --stream
ReinforcedIronPlate    | IronIngot  |
                       |            |
5 ReinforcedIronPlate  |            |
|-30 IronPlate         |            |
| |-45 IronIngot       |        45  |
|-60 Screw             |            |
| |-15 IronRod         |            |
| | |-15 IronIngot     |        15  |
                       |            |
Totals                 |        60
```

## Example 5

The `limit` parameter can be used to specify how many resources are available for a particular recipe.

```factory factory --stream
print ReinforcedIronPlate(limit 480 IronOre)
```

```#output factory --stream
ReinforcedIronPlate   
                      
40 ReinforcedIronPlate
|-240 IronPlate       
| |-360 IronIngot     
| | |-360 IronOre     
|-480 Screw           
| |-120 IronRod       
| | |-120 IronIngot   
| | | |-120 IronOre
```

## Example 6

The `alt` parameter can be used to specify what alternate recipes to use.

```factory factory --stream
print ReinforcedIronPlate(alt CastScrew, limit 60 IronOre)
```

```#output factory --stream
ReinforcedIronPlate  
                     
5 ReinforcedIronPlate
|-30 IronPlate       
| |-45 IronIngot     
| | |-45 IronOre     
|-60 CastScrew       
| |-15 IronIngot     
| | |-15 IronOre
```

## Example 7

Below are some examples combining everything above.

```factory factory --stream
print HeavyModularFrame(
  in IronIngot SteelIngot Concrete, 
  tally IronIngot SteelIngot Concrete, 
  tally inline IronIngot SteelIngot Concrete, 
  limit 480 IronIngot, 
  alt CastScrew)
```

```#output factory --stream
HeavyModularFrame           | IronIngot  | SteelIngot  | Concrete  |
                            |            |             |           |
3.2 HeavyModularFrame       |            |             |           |
|-16 ModularFrame           |            |             |           |
| |-24 ReinforcedIronPlate  |            |             |           |
| | |-144 IronPlate         |            |             |           |
| | | |-216 IronIngot       |       216  |             |           |
| | |-288 CastScrew         |            |             |           |
| | | |-72 IronIngot        |        72  |             |           |
| |-96 IronRod              |            |             |           |
| | |-96 IronIngot          |        96  |             |           |
|-64 SteelPipe              |            |             |           |
| |-96 SteelIngot           |            |         96  |           |
|-16 EncasedIndustrialBeam  |            |             |           |
| |-48 SteelBeam            |            |             |           |
| | |-192 SteelIngot        |            |        192  |           |
| |-96 Concrete             |            |             |       96  |
|-384 CastScrew             |            |             |           |
| |-96 IronIngot            |        96  |             |           |
                            |            |             |           |
Totals                      |       480  |        288  |       96  

IronIngot 480
SteelIngot 288
Concrete 96
```

## Example 8

```factory factory --stream
// The same recipe using IronPipes

print HeavyModularFrame(
  in IronIngot Concrete, 
  tally IronIngot Concrete, 
  limit 480 IronIngot, 
  alt CastScrew IronPipe)
```

```#output factory --stream
HeavyModularFrame             
                              
2.087 HeavyModularFrame       
|-10.435 ModularFrame         
| |-15.652 ReinforcedIronPlate
| | |-93.913 IronPlate        
| | | |-140.87 IronIngot      
| | |-187.826 CastScrew       
| | | |-46.957 IronIngot      
| |-62.609 IronRod            
| | |-62.609 IronIngot        
|-41.739 IronPipe             
| |-166.957 IronIngot         
|-10.435 EncasedIndustrialBeam
| |-31.304 SteelBeam          
| | |-125.217 SteelIngot      
| | | |-125.217 IronOre       
| | | |-125.217 Coal          
| |-62.609 Concrete           
|-250.435 CastScrew           
| |-62.609 IronIngot         

IronIngot 480
Concrete 62.609
```

# The recipe syntax

The code above is a bit cumbersome. We can rewrite it in a more readable format using the recipe 
syntax.

<span style="color:red">**NOTE:**</span> The recipe name must be different from the name of the item.

```factory factory --stream
recipe MyPlateRecipe
  out ReinforcedIronPlate
  in IronIngot

print MyPlateRecipe()

```

```#output factory --stream
MyPlateRecipe        
                     
1 ReinforcedIronPlate
|-6 IronPlate        
| |-9 IronIngot      
|-12 Screw           
| |-3 IronRod        
| | |-3 IronIngot
```

## Example 1

Combining the recipe syntax with the features above we can easily compare multiple versions of the 
same recipe.

```factory factory --stream
recipe MyHeavyFrameRecipe
  out HeavyModularFrame
  in IronIngot SteelIngot
  alt CastScrew
  tally IronIngot SteelIngot

print MyHeavyFrameRecipe(2) MyHeavyFrameRecipe(2, alt IronPipe EncasedIndustrialPipe)
```

```#output factory --stream
MyHeavyFrameRecipe        
                          
2 HeavyModularFrame       
|-10 ModularFrame         
| |-15 ReinforcedIronPlate
| | |-90 IronPlate        
| | | |-135 IronIngot     
| | |-180 CastScrew       
| | | |-45 IronIngot      
| |-60 IronRod            
| | |-60 IronIngot        
|-40 SteelPipe            
| |-60 SteelIngot         
|-10 EncasedIndustrialBeam
| |-30 SteelBeam          
| | |-120 SteelIngot      
| |-60 Concrete           
| | |-180 Limestone       
|-240 CastScrew           
| |-60 IronIngot         

IronIngot 300
SteelIngot 180

MyHeavyFrameRecipe        
                          
2 HeavyModularFrame       
|-10 ModularFrame         
| |-15 ReinforcedIronPlate
| | |-90 IronPlate        
| | | |-135 IronIngot     
| | |-180 CastScrew       
| | | |-45 IronIngot      
| |-60 IronRod            
| | |-60 IronIngot        
|-40 IronPipe             
| |-160 IronIngot         
|-10 EncasedIndustrialPipe
| |-60 IronPipe           
| | |-240 IronIngot       
| |-50 Concrete           
| | |-150 Limestone       
|-240 CastScrew           
| |-60 IronIngot         

IronIngot 700
SteelIngot 0
```

## Example 2

You can print the recipe definition by omitting the parens in 
the print statement.

```factory factory --stream
recipe MyHeavyFrameRecipe
  out HeavyModularFrame
  in IronIngot SteelIngot
  alt CastScrew
  tally IronIngot SteelIngot

print MyHeavyFrameRecipe
```

```#output factory --stream
recipe MyHeavyFrameRecipe
  in IronIngot SteelIngot
  out HeavyModularFrame
  alt CastScrew
  tally IronIngot SteelIngot
```