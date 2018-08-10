#Assembly Publicizer

## What is it for?

A tool to create a copy of an assembly in which all members are public (types, methods, fields, getters and setters of properties).  
The intended usage is for modding, because this way you can access everything normally without the use of reflection or some helper classes.  
If you use the modified publicized libary in your references and compile your dll with "Allow unsafe code", 
the access even works with the original assembly fine where the member still are private. (At least with Mono on Windows)  
This way you get the full features of your IDE, like auto completion and you don't have to worry about cumbersome stuff like 
creating an instance of an private nested class to use as an parameter for a private method.  
  
## Usage
You can drop your target dll onto the .exe (on Windows) or use the command line.  
The first parameter is the path to the target assembly (absolute or relative).  
The second parameter is optional and contains the output path and/or filename.  
* It can be just a (relative) path like "subdir1\subdir2"  
* It can be just a filename like "CustomFileName.dll"  
* It can be a filename with path like "C:\dir1\dir2\CustomFileName.dll"  
  If omited, it creates the modified assembly with an "_publicized" suffix in the subdirectory "publicized_assemblies".  
  This way it stays organized if you publicize multiple assemblies. 

## Support me
If you like my work, spread the word so more people can enjoy it.  
You also can show your appreciation with a metaphorical coffee or cabbage:  
<a href='https://ko-fi.com/Q5Q0BT8U' target='_blank'><img height='55' style='border:0px;height:55px;' 
src='https://github.com/CabbageCrow/Miscellaneous/blob/master/img/Kofi_btn/kofi_btn_coffee.png?v=0' border='0' alt='Buy Me a metaphorical Coffee at ko-fi.com' /></a> 
<a href='https://ko-fi.com/Q5Q0BT8U' target='_blank'><img height='55' style='border:0px;height:55px;' 
src='https://github.com/CabbageCrow/Miscellaneous/blob/master/img/Kofi_btn/kofi_btn_cabbage.png?v=0' border='0' alt='Give the Crow a Cabbage at ko-fi.com' /></a>
