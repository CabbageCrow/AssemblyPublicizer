# Assembly Publicizer

## What is it for?
  
A tool to create a copy of an assembly in which **all members are public** (types, methods, fields, getters and setters of properties).  
  
The intended usage is for modding in Unity(*), because this way you can **access everything normally without the use of reflection** or some helper classes.  
If you use the modified publicized libary in your references and **compile your dll with "Allow unsafe code" enabled**, 
the access even works with the original assembly fine where the member still are private.  
Without "Allow unsafe code" you get (sometimes?) an access violation exception during runtime when accessing private members except for types.  
This way you get the full features of your IDE, like **auto completion** and you don't have to worry about cumbersome stuff like 
creating an instance of an private nested class to use as an parameter for a private method.  
  
(*) It probably works for other instance than Unity too, maybe it's dependent if the software/game uses Mono? If you know more about it I would be happy to hear about it. :-)
  
## Usage
You can drop your target dll onto the .exe (on Windows) or use the command line.  
The **arguments** are the path to each **target assembly** (absolute or relative).  
  
### How to "Allow unsafe code" in Visual Studio
See the following link:  
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/unsafe-compiler-option  
  
## Support the original author
If you like it, spread the word so more people can enjoy it.  
You also can show your appreciation with a metaphorical coffee or cabbage:  
<a href='https://ko-fi.com/Q5Q0BT8U' target='_blank'><img height='55' style='border:0px;height:55px;' 
src='https://github.com/CabbageCrow/Miscellaneous/blob/master/img/Kofi_btn/kofi_btn_coffee.png?v=0' border='0' alt='Buy Me a metaphorical Coffee at ko-fi.com' /></a> 
<a href='https://ko-fi.com/Q5Q0BT8U' target='_blank'><img height='55' style='border:0px;height:55px;' 
src='https://github.com/CabbageCrow/Miscellaneous/blob/master/img/Kofi_btn/kofi_btn_cabbage.png?v=0' border='0' alt='Give the Crow a Cabbage at ko-fi.com' /></a>
