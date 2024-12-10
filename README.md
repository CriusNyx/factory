# Factory

Factory is a domain specific computer language designed to help you plan out your Satisfactory 
builds.

Factory's main application at this stage is to help you determine how many materials you need to 
produce a certain item. It can recursively solve most recipes in the game without aid.

It also lets you determine things like the ratio of items needed to implement a certain recipe or
alt recipe.

Factory is designed to have a grammar which is easy to learn and understand for non programers.

## Example

The following code computes how many resources are needed to produce 6 heavy modular frames per 
minute.

It uses the Cast Screw alternate recipe.

```factory factory --stream
recipe HMF
  out HeavyModularFrame
  in IronIngot SteelIngot Concrete
  tally IronIngot SteelIngot Concrete
  alt CastScrew

print HMF(6)
```

```#output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180
```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180```output factory --stream
HMF                       
                          
6 HeavyModularFrame       
|-30 ModularFrame         
| |-45 ReinforcedIronPlate
| | |-270 IronPlate       
| | | |-405 IronIngot     
| | |-540 CastScrew       
| | | |-135 IronIngot     
| |-180 IronRod           
| | |-180 IronIngot       
|-120 SteelPipe           
| |-180 SteelIngot        
|-30 EncasedIndustrialBeam
| |-90 SteelBeam          
| | |-360 SteelIngot      
| |-180 Concrete          
|-720 CastScrew           
| |-180 IronIngot        

IronIngot 900
SteelIngot 540
Concrete 180```

See the getting started page for more information.

## [Getting Started](./GettingStarted.md)
