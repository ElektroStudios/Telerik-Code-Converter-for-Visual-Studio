' ***********************************************************************
' Author   : Elektro
' Modified : 11-November-2017
' ***********************************************************************

#Region " Imports "

Imports DevCase.Core.Interop.CodeDOM

Imports EnvDTE80

Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Imports TelerikCodeConverterPackage.Enums
Imports TelerikCodeConverterPackage.Package

#End Region

Namespace TelerikCodeConverterPackage.Util

    Friend NotInheritable Class Converter

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="Converter"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Performs a code conversion operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="taskToPerform">
        ''' The code conversion task to perform.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' Returns zero if success, non-zero otherwise.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Async Function DoConvertAsync(dte As DTE2,
                                                    viewhost As IWpfTextViewHost,
                                                    taskToPerform As ConverterTask) As Task(Of Integer)

            Try

                Dim textView As IWpfTextView = viewhost.TextView
                Dim selection As ITextSelection = textView.Selection
                Dim language As String = CodeEditor.GetDocumentLanguage(dte)

                Dim telerikCodeConverterMethod As TelerikUtil.TelerikCodeConverterMethod
                Select Case language.ToUpper

                    Case "BASIC"
                        telerikCodeConverterMethod = TelerikUtil.TelerikCodeConverterMethod.VisualBasicToCSharp

                    Case "CSHARP"
                        telerikCodeConverterMethod = TelerikUtil.TelerikCodeConverterMethod.CSharpToVisualBasic

                    Case ""
                        ' Any document is open or else document without specific language.
                        Return -1

                    Case Else ' VC++, F#, etc...
                        Return -1

                End Select

                ' Build the code converter string.
                Select Case taskToPerform

                    Case ConverterTask.ConvertSelectedCode
                        Dim span As SnapshotSpan = selection.StreamSelectionSpan.SnapshotSpan
                        Dim text As String = span.GetText
                        If Not String.IsNullOrWhiteSpace(text) Then
                            Await Converter.TryConversionAsync(span.GetText, telerikCodeConverterMethod)
                        End If

                    Case ConverterTask.ConvertCurrentDocument
                        Dim objTextDoc As EnvDTE.TextDocument
                        Dim objEditPt As EnvDTE.EditPoint
                        Dim docText As String

                        ' Get a handle to the new document and create an EditPoint.
                        objTextDoc = DirectCast(dte.ActiveDocument.Object("TextDocument"), EnvDTE.TextDocument)
                        objEditPt = objTextDoc.StartPoint.CreateEditPoint

                        ' Get all Text of active document
                        docText = objEditPt.GetText(objTextDoc.EndPoint)
                        If Not String.IsNullOrWhiteSpace(docText) Then
                            Await Converter.TryConversionAsync(docText, telerikCodeConverterMethod)
                        End If

                    Case Else
                        Return -1

                End Select

                Return 0

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Telerik Code Converter Extension", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1

            End Try

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Tries the source-code conversion.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Async Function TryConversionAsync(sourceCode As String, telerikCodeConverterMethod As TelerikUtil.TelerikCodeConverterMethod) As Task

            Try
                sourceCode = Await TelerikUtil.TelerikCodeConvertAsync(telerikCodeConverterMethod, sourceCode)
                ' Print conversion result in the output pane.
                Main.OutputPane.Clear()
                Main.OutputPane.OutputStringThreadSafe(sourceCode & Environment.NewLine)
                Main.OutputPane.Activate()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unexpected error.", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try

            Dim openInExternalEditor As Boolean =
                CBool(Main.Dte.Properties("Telerik Code Converter", "Settings").Item("OpenInExternalEditor").Value)

            Dim externalEditorPath As String =
                CStr(Main.Dte.Properties("Telerik Code Converter", "Settings").Item("ExternalEditorPath").Value)

            If openInExternalEditor Then
                If String.IsNullOrWhiteSpace(externalEditorPath) Then
                    MessageBox.Show("The path to a external editor is empty. Please set the path, or deactivate the 'Open In External Editor' option.",
                                    "Telerik Code Converter", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Function
                End If

                If Not File.Exists(externalEditorPath) Then
                    MessageBox.Show("The external editor file is not found, please revise the 'External Editor Path' option.",
                                    "Telerik Code Converter", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Function
                End If

                Try
                    Dim filepath As String = ""
                    Select Case telerikCodeConverterMethod
                        Case TelerikUtil.TelerikCodeConverterMethod.CSharpToVisualBasic
                            filepath = Path.GetTempFileName & ".vb"
                        Case TelerikUtil.TelerikCodeConverterMethod.VisualBasicToCSharp
                            filepath = Path.GetTempFileName & ".cs"
                    End Select
                    File.WriteAllText(filepath, sourceCode, Encoding.Default)
                    Process.Start(externalEditorPath, filepath) ' C:\Program Files\Sublime Text\sublime_text.exe

                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Telerik Code Converter", MessageBoxButtons.OK, MessageBoxIcon.Error)

                End Try
            End If

            ' MessageBox.Show(result, "", MessageBoxButtons.OK, MessageBoxIcon.Information)

        End Function

#End Region

    End Class

End Namespace
