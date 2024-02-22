' ***********************************************************************
' Author   : Elektro
' Modified : 10-November-2017
' ***********************************************************************

#Region " ConverterTask "

Namespace TelerikCodeConverterPackage.Enums

    ''' <summary>
    ''' Specifies the Telerik Code Converter task to perform.
    ''' </summary>
    Friend Enum ConverterTask As Integer

        ''' <summary>
        ''' Convert selected code to C# or VB.NET.
        ''' </summary>
        ConvertSelectedCode = 0

        ''' <summary>
        ''' Convert current document to C# or VB.NET.
        ''' </summary>
        ConvertCurrentDocument = 1

    End Enum

End Namespace

#End Region
