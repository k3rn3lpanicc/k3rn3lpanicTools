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

# SystemInfo.cs
It will give you almost all the information you will need about your application and system
exp:
```C#
/*
public enum InfoType {
Machinename,
Username,
ApplicationFullPath,
ApplicationFullPath2,
CurrentDirectory,
OSversion,
SystemDirectory,
IsRunasADMIN,
AppName
}
 */
 string informationyouneed = k3rn3lpanicTools.SystemInfo.GetInfo(SystemInfo.InfoType.Itemfromlistabove:P);
 
 string[] args = k3rn3lpanicTools.SystemInfo.GetArgs(); //This will give you args that your file is called with
```

# Tools.cs
This is the best part
see the codes below : 
```C#
string randomfolder = k3rn3lpanicTools.Tools.GetRandomFolder(); //gives you a random and valid(with ok permissions) folder that is not in C drive

bool isitdone = k3rn3lpanicTools.Tools.ResetWindowsUserPassword(NewPass); //it will reset the password of windows to your string (Needs to be run as admin)

k3rn3lpanicTools.Tools.setInStartup(); //This will set your program in startup (with registry keys) and take care of it

List<Process> Whoislockingfile = k3rn3lpanicTools.Tools.WhoIsLocking(filename); //this will return a list of proccess that are using given file

bool isinstartup = k3rn3lpanicTools.Tools.IsInStartup(); //returns true if your program is in startup

k3rn3lpanicTools.Tools.ExecuteAsAdmin(filename); //This will run given file as administrator


Keys[] tobedisabled = {Keys.Control  , Keys.A}; //you can modify it =P
bool isitdone = k3rn3lpanicTools.Tools.disable_Keys(tobedisabled); //this will disable using given keys when app is running (keys will be disabled even outside the program window)
```
# Socket Programming
You can use classes Server and Client to run a server or connect to a server that runs with this library. right now you can send bytes(wich means you can send anything) but they are just shown as string. Sending file and reciving it will be a part of this classes soon.

# FTPClient.cs
usage : 
```C#
new FTPClient("ftpserver" , "username" , "password" , isUpload).Dowork("filenametoUploadorDownload");
```


# Comming Soon
Steganography,webscrab

<div align="center">
  
  <br><a>K3rn3lPanic</a> ??? <a href="mailto:k3rn3l.panic1832@gmail.com">Email</a> ??? <a href="https://github.com/k3rn3lpanicc">GitHub</a>
</div>
