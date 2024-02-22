' ***********************************************************************
' Author   : Elektro
' Modified : 11-November-2017
' ***********************************************************************

#Region " Command Ids "

Namespace TelerikCodeConverterPackage.Package

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Exposes the Command Identifiers for this package.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Module CommandIds

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Convert Selected Code to C# command id
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const ConvertSelectedCodeToCSharpId As UInteger = &H100

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Convert Selected Code to VB.NET command id
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const ConvertSelectedCodeToVBId As UInteger = &H101

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Convert Current Document to C# command id
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const ConvertCurrentDocumentToCSharpId As UInteger = &H102

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Convert Current Document to VB.NET command id
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const ConvertCurrentDocumentToVBId As UInteger = &H103

    End Module

End Namespace

#End Region
