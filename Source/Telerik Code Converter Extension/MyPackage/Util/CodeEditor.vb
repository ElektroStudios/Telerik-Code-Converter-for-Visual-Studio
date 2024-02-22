' ***********************************************************************
' Author   : Elektro
' Modified : 11-December-2015
' ***********************************************************************

#Region " Imports "

Imports EnvDTE
Imports EnvDTE80

Imports Microsoft.VisualStudio.Editor
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.TextManager.Interop

Imports System

Imports TelerikCodeConverterPackage.Package

#End Region

#Region " Code-Editor Tools "

Namespace TelerikCodeConverterPackage.Util

    Friend NotInheritable Class CodeEditor

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="CodeEditor"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        '''' ----------------------------------------------------------------------------------------------------
        '''' <summary>
        '''' Determines whether there is a selected text in the code window editor.
        '''' </summary>
        '''' ----------------------------------------------------------------------------------------------------
        '''' <param name="dte">
        '''' The <see cref="DTE2"/> instance. 
        '''' </param>
        '''' ----------------------------------------------------------------------------------------------------
        '''' <returns>
        '''' <c>True</c> if there is selected text, <c>False</c> otherwise.
        '''' </returns>
        '''' ----------------------------------------------------------------------------------------------------
        'Friend Shared Function IsTextSelected(dte As DTE2) As Boolean

        '    Return Not GetCurrentViewHost.TextView.Selection.IsEmpty

        'End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether there is a selected text in the code window editor.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dte">
        ''' The <see cref="DTE2"/> instance. 
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <c>True</c> if there is selected text, <c>False</c> otherwise.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function IsTextSelected() As Boolean

            Return Not GetCurrentViewHost.TextView.Selection.IsEmpty

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current view host.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see cref="IWpfTextViewHost"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function GetCurrentViewHost() As IWpfTextViewHost

            Dim txtMgr As IVsTextManager = DirectCast(Main.GetGlobalService(GetType(SVsTextManager)), IVsTextManager)
            Dim vTextView As IVsTextView = Nothing
            Dim mustHaveFocus As Integer = 1

            txtMgr.GetActiveView(mustHaveFocus, Nothing, vTextView)

            Dim userData As IVsUserData = TryCast(vTextView, IVsUserData)

            If userData Is Nothing Then
                Return Nothing

            Else
                Dim viewHost As IWpfTextViewHost
                Dim holder As Object = Nothing
                Dim guidViewHost As Guid = DefGuidList.guidIWpfTextViewHost
                userData.GetData(guidViewHost, holder)
                viewHost = DirectCast(holder, IWpfTextViewHost)
                Return viewHost

            End If

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the document language.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dte">
        ''' The <see cref="DTE2"/> instance. 
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The language name like "BASIC" or "CSHARP". 
        ''' If there is any document open or else an open document without specific language, then it returns an empty string.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function GetDocumentLanguage(ByVal dte As DTE2) As String

            If dte.ActiveWindow.Document IsNot Nothing Then

                Dim activeDoc As Document = dte.ActiveDocument

                Dim langString As String = activeDoc.Language

                Return If(String.IsNullOrEmpty(langString), "", langString)

            Else
                Return ""

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region
