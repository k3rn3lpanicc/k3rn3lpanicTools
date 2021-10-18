# k3rn3lpanicTools
 The most useful and necessary Library in C# I Created :P

# AsymmetricProvider.cs and Encryption.cs
They are used for Encryption of course =P
Some Examples : 
```C#
//AES File Encryption\Decryption:
k3rn3lpanicTools.Encryption.FileEnc.AES_EncryptFile(sourceFilename , destinationFilename , password , iterations);
k3rn3lpanicTools.Encryption.FileEnc.AES_DecryptFile(sourceFilename, destinationFilename, password, iterations);


//AES string Encryption\Decryption:
string cipherText = k3rn3lpanicTools.Encryption.StringEnc.AES_Encryptstr(clearText, password);
string decrypted = k3rn3lpanicTools.Encryption.StringEnc.AES_Decryptstr(cipherText, password);


//MD5 Hashing :
string Hashed = k3rn3lpanicTools.Encryption.Hashing.getMD5(data);


//AsymmetricProvider :
var Keys = k3rn3lpanicTools.AsymmetricProvider.GenerateNewKeyPair(); //Creates a Key Pair for encryption
//string part :
string enc = k3rn3lpanicTools.AsymmetricProvider.EncryptString(value, Keys.PublicKey);
string dec = k3rn3lpanicTools.AsymmetricProvider.DecryptString(value, Keys.PrivateKey);
//File part :
k3rn3lpanicTools.AsymmetricProvider.EncryptFile(inputFilePath, outputFilePath, Keys.PublicKey);
k3rn3lpanicTools.AsymmetricProvider.DecryptFile(inputFilePath, outputFilePath, Keys.PrivateKey);
```

# Archive.cs
It is used to make Zip files out of folders or Extract them
Example :
```C#
k3rn3lpanicTools.Archive.ZipFolder(FolderPath,"Zipfile.zip");
k3rn3lpanicTools.Archive.UnZipFolder("Zipfile.zip", Decrypted_FolderName);
```

# Compiler.cs
It is used to compile a c# script from file or straight from string
Example :
```C#
//Run after compile or not ?:
bool runafterCompile = true;

//Compile from a c# file : 
k3rn3lpanicTools.Compiler.compileCsharpFile(Outputfilename,file,runafterCompile);
//Compile from string : 
k3rn3lpanicTools.Compiler.compileCsharpCode(Outputfilename, code, runafterCompile);
```
# FileASSOC.cs
It is used to set Default program for a specific type of file (for example you can set default program of files \*.mlf to your program) , and to get the program that runs a specific type of format
Example:
```C#
//setting the default program of running a .lf file :
string myExecuteable = k3rn3lpanicTools.SystemInfo.GetInfo(SystemInfo.InfoType.ApplicationFullPath);
k3rn3lpanicTools.FileASSOC.SetAssociation_User("lf", myExecuteable, "Test Lib.exe");

//getting the default program that runs .lf files :
string Expath = k3rn3lpanicTools.FileASSOC.FindAssocExecuteable("myfile.lf");

//Check if there is a defualt program of a specific format or not :
bool hasit k3rn3lpanicTools.FileASSOC.HasExecutable(file);
```


