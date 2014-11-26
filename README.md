# NetMFAMS
===========================

A class library for communicating from .NET Micro Framework with Azure Mobile Services. Right now this only works with the .NET Backend version of Azure Mobile Services but I am working also on the Javascript Backend.

### Credits

This library is a forked and modified version of uCloudy (https://ucloudy.codeplex.com/) so thanks for this great work. There are also some parts of (https://github.com/highfield/azure-veneziano) so thanks for that to.


### Requirements
---

Microsoft .NET Micro Framework 4.2 or higher



### Dependencies
---

This library depends on Json.NetMF (https://github.com/mweimer/Json.NetMF), so credits to this library


### Example Usage

You have to instantiate the Class with the constructor and provide at minimum the URL of the Mobile Services. The second and third parameter are the Application and Master Key which you need for CRUD operations over the Internet.

```c#
var mobileService = new MobileServiceClient(<Your Mobile Service Url>,<ApplicationKey>,<MasterKey>);
```

You also need a Class of your data which implements the IMobileServiceEntityData Interface:

```c#
  public class GadgeteerData : IMobileServiceEntityData
  {
      public double Temperature { get; set; }
      public string Id { get; set; }
  }
```

Now you could start with the CRUD operations:

##### Insert:

Create an instance of your Data/Entity and then insert it into your Table. You will get back the Id which was created by the Mobile Service Backend automatically.

```c#
var gadgeteerData = new GadgeteerData
{
  Temperature = 24.2
}

var result = mobileService.Insert("<Tablename>",gadgeteerData);
gadgeteerData.Id = result;
```

##### Update:
Change the instance of your Data/Entity and then insert into your Table. It is important that the Id is the correct one which you generated or the Mobile Service generated because I need the Id to get the correct URL. You will get back the HTTP Status Code of the request. 

```c#
gadgeteerData.Temperature = 18.2;

if (mobileService.Update("<Tablename>",gadgeteerData) == HttpStatusCode.OK)
{
  Debug.Print("Success");
}
```

##### Delete:
Send the Id of your Data/Entity to the Mobile Service and it will be deleted.

```c#
if (mobileService.Update("<Tablename>",gadgeteerData.Id) == HttpStatusCode.NoContent)
{
  Debug.Print("Success");
}
```

##### Query:
Send only the Table Name of your Mobile Services Table and you will get back all of your Entities inside a JSON string. You could also add a query to the request. Here is a good overlook which queries you could send: http://msdn.microsoft.com/en-us/library/azure/jj677199.aspx

```c#
var result = mobileService.Query("<Tablename>")
Debug.Print(result);
```

