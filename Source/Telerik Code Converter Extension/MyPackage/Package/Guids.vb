' ***********************************************************************
' Author   : Elektro
' Modified : 11-November-2017
' ***********************************************************************

#Region " Guids "

Imports System

Namespace TelerikCodeConverterPackage.Package

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Exposes the package info, such as GUIDs and Command Identifiers.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Module Guids

#Region " Package "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The unique GUID identifier of this package, as String datatype.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const PackageGuidString As String = "A833BB60-94D5-4496-BCA7-D60AA4DBBB5A"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The unique GUID identifier of this package.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend ReadOnly Package As New Guid(Guids.PackageGuidString)

#End Region

#Region " Command-sets "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The unique identifier of the "Telerik Code Converter" commands set.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend ReadOnly CmdConverter As New Guid("51364FE7-1332-4070-859D-1EC91CA2F406")

#End Region

    End Module

End Namespace

#End Region
