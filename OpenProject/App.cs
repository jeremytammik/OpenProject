#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.Reflection;
#endregion

namespace OpenProject
{
  class App : IExternalApplication
  {
    const string _test_project_filepath
      = "Z:/a/rvt/CurvedWall.rvt";

    //UIApplication _uiapp;

    public Result OnStartup( UIControlledApplication a )
    {
      #region Retrieving UIApplication from UIControlledApplication
      // These attempts to access a UIApplication
      // or Application instance are all in vain:
      //
      //_uiapp = (UIApplication) a;
      //_uiapp = (UIApplication) a.ControlledApplication;
      //Application app = (Application) a;
      //Application app2 = (Application) a.ControlledApplication;
      //Application app3 = a.m_application;

      // Using Reflection works, though:

      Type type = a.GetType();

      // Not useful in this case, but interesting:

      MemberInfo[] publicMembers = type.GetMembers();
      MemberInfo[] nonPublicMembers = type.GetMembers( BindingFlags.NonPublic );
      MemberInfo[] staticMembers = type.GetMembers( BindingFlags.Static );

      // This is the call that finally yields useful results:

      BindingFlags flags = BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.GetProperty
        | BindingFlags.Instance;

      MemberInfo[] propertyMembers = type.GetMembers(
        flags );

      // Note that the field "m_application" is listed
      // in the propertyMembers array, and also the 
      // method "getUIApp"... let's grab the field:

      string propertyName = "m_application";
      flags = BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.GetField | BindingFlags.Instance;
      Binder binder = null;
      object[] args = null;

      object result = type.InvokeMember(
          propertyName, flags, binder, a, args );

      UIApplication _uiapp;

      _uiapp = (UIApplication) result;
      #endregion // Retrieving UIApplication from UIControlledApplication

      a.ControlledApplication.ApplicationInitialized
        += OnApplicationInitialized;

      return Result.Succeeded;
    }

    void OnApplicationInitialized(
      object sender,
      ApplicationInitializedEventArgs e )
    {
      // This does not work, because the sender is
      // an Application instance, not UIApplication.

      //UIApplication uiapp = sender as UIApplication;

      // Sender is an Application instance:

      Application app = sender as Application;

      // However, UIApplication can be 
      // instantiated from Application.

      UIApplication uiapp = new UIApplication( app );

      uiapp.OpenAndActivateDocument(
        _test_project_filepath );
    }

    public Result OnShutdown( UIControlledApplication a )
    {
      return Result.Succeeded;
    }
  }
}
