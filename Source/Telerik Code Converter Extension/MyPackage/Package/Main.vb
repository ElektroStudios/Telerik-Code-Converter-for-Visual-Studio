' ***********************************************************************
' Author   : Elektro
' Modified : 11-November-2017
' ***********************************************************************

#Region " Imports "

Imports EnvDTE80

Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Text.Editor

Imports System
Imports System.ComponentModel.Design
#If DEBUG Then
Imports System.Diagnostics
#End If
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows

Imports TelerikCodeConverterPackage.Enums
Imports TelerikCodeConverterPackage.UserInterface
Imports TelerikCodeConverterPackage.Util

#End Region

Namespace TelerikCodeConverterPackage.Package

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' This is the class that implements the package exposed by this assembly.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' The <see cref="PackageRegistrationAttribute"/> attribute tells the 
    ''' PkgDef creation utility (CreatePkgDef.exe) that this class is a package.
    ''' <para></para>
    ''' 
    ''' The <see cref="InstalledProductRegistrationAttribute"/> attribute is used to 
    ''' register the information needed to show this package in the Help/About dialog of Visual Studio.
    ''' <para></para>
    ''' 
    ''' The <see cref="ProvideMenuResourceAttribute"/> attribute is needed to 
    ''' let the shell know that this package exposes some menus.
    ''' <para></para>
    ''' 
    ''' The <see cref="ProvideAutoLoadAttribute"/> attribute is needed to auto-load the package when the specified condition occurs.
    ''' <para></para>
    ''' 
    ''' The <see cref="ProvideOptionPageAttribute"/> attribute is needed to provide the options page under "Tools -> Options" menu.
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400)>
    <ProvideMenuResource("Menus.ctmenu", 1)>
    <Guid(Guids.PackageGuidString)>
    <ProvideOptionPage(GetType(OptionsPage), "Telerik Code Converter", "Settings", 0, 0, True)>
    <ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)>
    <PackageRegistration(UseManagedResourcesOnly:=True, AllowsBackgroundLoading:=True)>
    Public NotInheritable Class Main : Inherits Shell.AsyncPackage

#Region " Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="EnvDTE80.DTE2"/> instance. 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Dte As DTE2

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="DteInitializer"/> instance that initializes <see cref="Main.Dte"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend DteInitializer As DteInitializer

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The output pane where to print the resulting code conversions.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared OutputPane As IVsOutputWindowPane

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The GUID unique identifier for <see cref="Main.OutputPane"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private OutputPaneGuid As Guid

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The command to Convert selected code to C#.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private WithEvents CmdConvertSelectedCodeToCSharp As OleMenuCommand

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The command to Convert selected code to VB.NET.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private WithEvents CmdConvertSelectedCodeToVB As OleMenuCommand

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The command to convert the current document to C#.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private WithEvents CmdConvertCurrentDocumentToCSharp As OleMenuCommand

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The command to convert the current document to VB.NET.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private WithEvents CmdConvertCurrentDocumentToVB As OleMenuCommand

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Main"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Sub New()

            ' Inside this method you can place any initialization code that does not require 
            ' any Visual Studio service because at this point the package object is created but 
            ' not sited yet inside Visual Studio environment. 

#If DEBUG Then
            Debug.WriteLine(String.Format("Entering constructor for: {0}", Me.GetType.Name))
#End If

        End Sub



#End Region
#Region " Methods "
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Called when the VSPackage is loaded by Visual Studio.
        ''' <para></para>
        ''' This is the place where you can put all the initialization code that rely on services provided by VisualStudio.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Async Function InitializeAsync(cancellationToken As CancellationToken, progress As IProgress(Of ServiceProgressData)) As Task

#If DEBUG Then
            Debug.WriteLine(String.Format("Entering Initialize() of: {0}", Me.GetType().Name))
#End If
            MyBase.Initialize()

            ' When initialized asynchronously, the current thread may be a background thread at this point.
            ' Do any initialization that requires the UI thread after switching to the UI thread.
            Await Me.JoinableTaskFactory.SwitchToMainThreadAsync()

            Me.InitializeDte()

            If (Main.Dte IsNot Nothing) Then
                Me.InitializeMenuHandlers()
            End If

            Me.OutputPaneGuid = New Guid("FFE5FB7B-8406-49E4-9E16-5B0401A63AF1")
            Me.CreateOutputPane(Me.OutputPaneGuid, True, True, "Telerik Code Converter")

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sample code to get an instance of the <see cref="EnvDTE.DTE"/> root of the automation model.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://www.mztools.com/articles/2013/MZ2013029.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub InitializeDte()

            Dim shellService As IVsShell

            Main.Dte = TryCast(Me.GetService(GetType(SDTE)), DTE2)

            If Main.Dte Is Nothing Then
                ' The IDE is not yet fully initialized.
                shellService = TryCast(Me.GetService(GetType(SVsShell)), IVsShell)
                Me.DteInitializer = New DteInitializer(shellService, AddressOf Me.InitializeDte)

            Else
                ' Main.Dte.UserControl = False
                Me.DteInitializer = Nothing

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initialized the menu handlers.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub InitializeMenuHandlers()

            ' Add our command handlers for menu (commands must exist in the .vsct file)
            Dim mcs As OleMenuCommandService = TryCast(Me.GetService(GetType(IMenuCommandService)), OleMenuCommandService)

            If (mcs IsNot Nothing) Then

                Dim cmdIdConvertSelectedCodeToCSharp As New CommandID(Guids.CmdConverter, CommandIds.ConvertSelectedCodeToCSharpId)
                Me.CmdConvertSelectedCodeToCSharp = New OleMenuCommand(AddressOf Me.CmdConvertSelectedCode_Callback, cmdIdConvertSelectedCodeToCSharp)

                Dim cmdIdConvertSelectedCodeToVB As New CommandID(Guids.CmdConverter, CommandIds.ConvertSelectedCodeToVBId)
                Me.CmdConvertSelectedCodeToVB = New OleMenuCommand(AddressOf Me.CmdConvertSelectedCode_Callback, cmdIdConvertSelectedCodeToVB)

                Dim cmdIdConvertCurrentDocumentToCSharp As New CommandID(Guids.CmdConverter, CommandIds.ConvertCurrentDocumentToCSharpId)
                Me.CmdConvertCurrentDocumentToCSharp = New OleMenuCommand(AddressOf Me.CmdConvertCurrentDocument_Callback, cmdIdConvertCurrentDocumentToCSharp)

                Dim cmdIdConvertCurrentDocumentToVB As New CommandID(Guids.CmdConverter, CommandIds.ConvertCurrentDocumentToVBId)
                Me.CmdConvertCurrentDocumentToVB = New OleMenuCommand(AddressOf Me.CmdConvertCurrentDocument_Callback, cmdIdConvertCurrentDocumentToVB)

                With mcs
                    .AddCommand(Me.CmdConvertSelectedCodeToCSharp)
                    .AddCommand(Me.CmdConvertSelectedCodeToVB)
                    .AddCommand(Me.CmdConvertCurrentDocumentToCSharp)
                    .AddCommand(Me.CmdConvertCurrentDocumentToVB)
                End With

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the output pane where to print the resulting code conversions.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="paneGuid">
        ''' The pane unique identifier.
        ''' </param>
        ''' 
        ''' <param name="visible">
        ''' If set to <see langword="True"/>, the created pane is initially visible.
        ''' </param>
        ''' 
        ''' <param name="clearWithSolution">
        ''' If set to <see langword="True"/>, the output of the created pane is cleared when the solution closes.
        ''' </param>
        ''' 
        ''' <param name="title">
        ''' The pane title.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub CreateOutputPane(paneGuid As Guid, visible As Boolean, clearWithSolution As Boolean, title As String)

            Dim output As IVsOutputWindow = DirectCast(Me.GetService(GetType(SVsOutputWindow)), IVsOutputWindow)
            output.CreatePane(paneGuid, title, Convert.ToInt32(visible), Convert.ToInt32(clearWithSolution))

            ' Retrieve the created pane.  
            output.GetPane(paneGuid, Main.OutputPane)

        End Sub

#End Region

#Region " Command CallBacks "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="Main.CmdConvertSelectedCodeToCSharp"/> and <see cref="Main.CmdConvertSelectedCodeToVB"/> 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Async Sub CmdConvertSelectedCode_Callback(ByVal sender As Object, ByVal e As EventArgs)

            Dim view As IWpfTextViewHost = CodeEditor.GetCurrentViewHost
            '  Dim result As Integer
            Await Converter.DoConvertAsync(Main.Dte, view, ConverterTask.ConvertSelectedCode)
            '  ErrorHandler.ThrowOnFailure(result, {0, &HFFFFFFFF})

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="Main.CmdConvertCurrentDocumentToCSharp"/> and <see cref="Main.CmdConvertCurrentDocumentToVB"/> 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Async Sub CmdConvertCurrentDocument_Callback(ByVal sender As Object, ByVal e As EventArgs)

            Dim view As IWpfTextViewHost = CodeEditor.GetCurrentViewHost
            ' Dim result As Integer
            Await Converter.DoConvertAsync(Main.Dte, view, ConverterTask.ConvertCurrentDocument)
            ' ErrorHandler.ThrowOnFailure(result, {0, &HFFFFFFFF})

        End Sub

#End Region

#Region " Event-Handlers "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Handles the <see cref="OleMenuCommand.BeforeQueryStatus"/> event 
        ''' of the <see cref="Main.CmdConvertSelectedCodeToCSharp"/> 
        ''' and <see cref="Main.CmdConvertSelectedCodeToVB"/> commands.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub CmdConvertSelectedCode_BeforeQueryStatus(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CmdConvertSelectedCodeToCSharp.BeforeQueryStatus,
                CmdConvertSelectedCodeToVB.BeforeQueryStatus

            Dim cmd As OleMenuCommand = DirectCast(sender, OleMenuCommand)

            Select Case cmd.CommandID.ID

                Case CommandIds.ConvertSelectedCodeToCSharpId
                    cmd.Visible = (CodeEditor.GetDocumentLanguage(Main.Dte).ToUpper() = "BASIC")

                Case CommandIds.ConvertSelectedCodeToVBId
                    cmd.Visible = (CodeEditor.GetDocumentLanguage(Main.Dte).ToUpper() = "CSHARP")

            End Select

            cmd.Enabled = (cmd.Visible) AndAlso CodeEditor.IsTextSelected()

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Handles the <see cref="OleMenuCommand.BeforeQueryStatus"/> event 
        ''' of the <see cref="Main.CmdConvertCurrentDocumentToCSharp"/> 
        ''' and <see cref="Main.CmdConvertCurrentDocumentToVB"/> commands.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub CmdConvertCurrentDocumentToCSharp_BeforeQueryStatus(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CmdConvertCurrentDocumentToCSharp.BeforeQueryStatus,
                CmdConvertCurrentDocumentToVB.BeforeQueryStatus

            Dim cmd As OleMenuCommand = DirectCast(sender, OleMenuCommand)

            Select Case cmd.CommandID.ID

                Case CommandIds.ConvertCurrentDocumentToCSharpId
                    cmd.Visible = (CodeEditor.GetDocumentLanguage(Main.Dte).ToUpper() = "BASIC")

                Case CommandIds.ConvertCurrentDocumentToVBId
                    cmd.Visible = (CodeEditor.GetDocumentLanguage(Main.Dte).ToUpper() = "CSHARP")

            End Select

            If (cmd.Visible) Then
                Dim objTextDoc As EnvDTE.TextDocument
                Dim objEditPt As EnvDTE.EditPoint
                Dim docText As String

                ' Get a handle to the new document and create an EditPoint.
                objTextDoc = DirectCast(Dte.ActiveDocument.Object("TextDocument"), EnvDTE.TextDocument)
                objEditPt = objTextDoc.StartPoint.CreateEditPoint

                ' Get all Text of active document
                docText = objEditPt.GetText(objTextDoc.EndPoint)

                cmd.Enabled = Not String.IsNullOrWhiteSpace(docText)
            Else
                cmd.Enabled = False
            End If

        End Sub

#End Region

#Region " IDisposable Implementation "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Releases the resources used by the <see cref="Shell.Package"/> object.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="disposing">
        ''' <see langword="True"/> if the object is being disposed, <see langword="False"/> if it is being finalized.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)

            If (disposing) Then
                Main.Dte = Nothing
                Main.OutputPane = Nothing
                Me.DteInitializer = Nothing
                Me.OutputPaneGuid = Nothing
                Me.CmdConvertCurrentDocumentToCSharp = Nothing
                Me.CmdConvertCurrentDocumentToVB = Nothing
                Me.CmdConvertSelectedCodeToCSharp = Nothing
                Me.CmdConvertSelectedCodeToVB = Nothing
            End If

            MyBase.Dispose(disposing)

        End Sub

#End Region

    End Class

End Namespace
