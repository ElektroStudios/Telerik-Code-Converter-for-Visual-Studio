' ***********************************************************************
' Author   : Elektro
' Modified : 11-November-2017
' ***********************************************************************

#Region " Imports "

Imports Microsoft.VisualStudio.Shell

Imports System
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Imports TelerikCodeConverterPackage.Package

#End Region

#Region " Options Page "

Namespace TelerikCodeConverterPackage.UserInterface

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The Options page of the extension.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/en-us/library/bb166195.aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <DesignerCategory("Code")>
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <CLSCompliant(False), ComVisible(True)>
    Public NotInheritable Class OptionsPage : Inherits DialogPage

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="OptionsPage"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
        End Sub

#End Region

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value indicating whether the converted code should be opened in a external editor.
        ''' <para></para>
        ''' See also <see cref="OptionsPage.ExternalEditorPath"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Category("Settings")>
        <DisplayName("Open In External Editor")>
        <Description("If enabled, opens the converted code in the specified external editor.")>
        Public Property OpenInExternalEditor As Boolean = False

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the full path to a external code editor (eg. 'C:\Windows\Notepad.exe').
        ''' <para></para>
        ''' See also <see cref="OptionsPage.OpenInExternalEditor"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Category("Settings")>
        <DisplayName("External Editor Path")>
        <Description("Specifies the full path to a external code editor.")>
        Public Property ExternalEditorPath As String = "C:\Windows\Notepad.exe"

#End Region

    End Class

End Namespace

#End Region
