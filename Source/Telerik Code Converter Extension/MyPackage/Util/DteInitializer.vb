' ***********************************************************************
' Author   : Elektro
' Modified : 11-December-2015
' ***********************************************************************

#Region " Imports "

Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop

Imports System

#End Region

#Region " DTE Initializer "

Namespace TelerikCodeConverterPackage.Util

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes DTE for the current package's IDE instance.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="http://www.mztools.com/articles/2013/MZ2013029.aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class DteInitializer : Implements IVsShellPropertyEvents

#Region " Private Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Reference to an <see cref="IVsShell"/> instance,
        ''' which provides access to the fundamental environment services, 
        ''' specifically those dealing with VSPackages and the registry.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private ReadOnly shellService As IVsShell

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The callback delegate.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private ReadOnly callback As Action

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Abstract handle used to unadvise the client of property changes to the environment.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private cookie As UInteger

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="Converter"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="DteInitializer"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="shellService">
        ''' The <see cref="IVsShell"/> instance.
        ''' </param>
        ''' 
        ''' <param name="callback">
        ''' The callback delegate.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Sub New(ByVal shellService As IVsShell,
                       ByVal callback As Action)

            Dim hr As Integer

            Me.shellService = shellService
            Me.callback = callback

            ' Set an event handler to detect when the IDE is fully initialized.
            hr = Me.shellService.AdviseShellPropertyChanges(Me, Me.cookie)

            ErrorHandler.ThrowOnFailure(hr)

        End Sub

#End Region

#Region " IVsShellPropertyEvents Implementations "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Called when a shell property changes.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="propid">
        ''' ID of the property that changed.
        ''' </param>
        ''' 
        ''' <param name="var">
        ''' The new value of the property.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the method succeeds, it returns <see cref="VSConstants.S_OK"/>.
        ''' <para></para>
        ''' If it fails, it returns an error code.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Private Function IVsShellPropertyEvents_OnShellPropertyChange(ByVal propid As Integer,
                                                                      ByVal var As Object) As Integer Implements IVsShellPropertyEvents.OnShellPropertyChange

            Dim hr As Integer
            Dim isZombie As Boolean

            If (propid = __VSSPROPID.VSSPROPID_Zombie) Then

                isZombie = CBool(var)

                If Not isZombie Then

                    ' Release the event handler to detect when the IDE is fully initialized.
                    hr = Me.shellService.UnadviseShellPropertyChanges(Me.cookie)

                    ErrorHandler.ThrowOnFailure(hr)
                    Me.cookie = 0
                    Me.callback()

                End If

            End If

            Return VSConstants.S_OK

        End Function

#End Region

    End Class

End Namespace

#End Region
