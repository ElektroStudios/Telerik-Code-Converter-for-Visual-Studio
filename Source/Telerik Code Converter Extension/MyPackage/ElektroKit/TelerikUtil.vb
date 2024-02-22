





' THIS OPEN-SOURCE APPLICATION IS POWERED BY DEVCASE CLASS LIBRARY BY ELEKTRO STUDIOS.

' IF YOU LIKED THIS FREE APPLICATION, THEN PLEASE CONSIDER TO BUY DEVCASE CLASS LIBRARY FOR .NET AT:
' https://codecanyon.net/item/DevCase-class-library-for-net/19260282

' YOU CAN FIND THESE HELPER METHODS AND A MASSIVE AMOUNT MORE!, 
' +850 EXTENSION METHODS FOR ALL KIND OF TYPES, CUSTOM USER-CONTROLS, 
' EVERYTHING FOR THE NEWBIE And THE ADVANCED USER, FOR VB.NET AND C#. 

' DevCase is a utility framework containing new APIs and extensions to the core .NET Framework 
' to help complete your developer toolbox.
' It Is a set of general purpose classes provided as easy to consume packages.
' These utility classes and components provide productivity in day to day software development 
' mainly focused To WindowsForms. 

' UPDATES OF DevCase ARE MAINTAINED AND RELEASED EVERY MONTH.





#Region " Imports "
Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading.Tasks

Imports Newtonsoft.Json

#End Region

#Region " DevCase.Core.Interop.CodeDOM.TelerikUtil "

Namespace DevCase.Core.Interop.CodeDOM

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains related utilities to Telerik's Code Converter web service.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class TelerikUtil

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="TelerikUtil"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Specifies a conversion method for an http response to <see href="http://converter.telerik.com/service.asmx"/> web service.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Enum TelerikCodeConverterMethod As Integer

            ''' <summary>
            ''' Converts a C-Sharp source-code to its equivalent Visual Basic.NET code.
            ''' </summary>
            CSharpToVisualBasic

            ''' <summary>
            ''' Converts a Visual Basic.NET source-code to its equivalent C-Sharp code.
            ''' </summary>
            VisualBasicToCSharp

        End Enum

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Converts source-codes between C# and Visual Basic.NET languages.
        ''' <para></para>
        ''' This feature is powered by Telerik's Code converter web service (<see href="http://converter.telerik.com/service.asmx"/>).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' http://converter.telerik.com/service.asmx
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="converterMethod">
        ''' The conversion method, that is, VB.NET to C#, or C# to VB.NET.
        ''' </param>
        ''' 
        ''' <param name="sourceCode">
        ''' The source-code to convert.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting source-code converted in the specified language by <paramref name="converterMethod"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidEnumArgumentException">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Async Function TelerikCodeConvertAsync(converterMethod As TelerikCodeConverterMethod, sourceCode As String) As Task(Of String)

            Const apiUrl As String = "https://converter.telerik.com/api/converter/"

            Dim requestData As String = JsonConvert.SerializeObject(New With {
                Key .code = sourceCode,
                Key .requestedConversion =
                        If(converterMethod = TelerikCodeConverterMethod.CSharpToVisualBasic,
                        "cs2vbnet",
                        "vbnet2cs")
            })

            Dim byteData As Byte() = Encoding.UTF8.GetBytes(requestData)
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.ContentLength = byteData.Length

            Try
                Using stream As Stream = request.GetRequestStream()
                    Await stream.WriteAsync(byteData, 0, byteData.Length)
                End Using

                Using response As HttpWebResponse = CType(Await request.GetResponseAsync(), HttpWebResponse)
                    Using reader As New StreamReader(response.GetResponseStream())
                        Dim jsonResponse As String = reader.ReadToEnd()
                        Dim conversionResponse As ConversionResponse =
                            JsonConvert.DeserializeObject(Of ConversionResponse)(jsonResponse)

                        Return If(Not String.IsNullOrWhiteSpace(conversionResponse.ErrorMessage),
                            conversionResponse.ErrorMessage,
                            conversionResponse.ConvertedCode)
                    End Using
                End Using

            Catch ex As WebException
                Return $"Error: {ex.Message}"

            Catch ex As Exception
                Return $"Error: {ex.Message}"

            End Try

            ' OLD (OBSOLETED) METHODOLOGY.
            ' ----------------------------
            ' https://converter.telerik.com/service.asmx
            ' IT WORKS FOR "ConvertToVB" METHOD,
            ' BUT IT DOES NOR WORK FOR "ConvertToCS" METHOD.
            '
            'Dim xml As XDocument
            '
            'Select Case converterMethod
            '
            '    Case TelerikCodeConverterMethod.CSharpToVisualBasic
            '        xml = <?xml version="1.0" encoding="utf-8"?>
            '              <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            '                  xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            '                  xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
            '                  <soap12:Body>
            '                      <ConvertToVB xmlns="http://converter.telerik.com">
            '                          <cSharpCode><%= New XCData(sourceCode) %></cSharpCode>
            '                      </ConvertToVB>
            '                  </soap12:Body>
            '              </soap12:Envelope>
            '
            '    Case TelerikCodeConverterMethod.VisualBasicToCSharp
            '        xml = <?xml version="1.0" encoding="utf-8"?>
            '              <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            '                  xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            '                  xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
            '                  <soap12:Body>
            '                      <ConvertToCS xmlns="http://converter.telerik.com">
            '                          <vbCode><%= New XCData(sourceCode) %></vbCode>
            '                      </ConvertToCS>
            '                  </soap12:Body>
            '              </soap12:Envelope>
            '
            '    Case Else
            '        Throw New InvalidEnumArgumentException(argumentName:="converterMethod",
            '                                               invalidValue:=converterMethod,
            '                                               enumClass:=GetType(TelerikCodeConverterMethod))
            '
            'End Select
            '
            'Dim result As XDocument =
            '  Await WebUtil.PostXmlHttpRequestAsync(New Uri("https://converter.telerik.com/service.asmx"), xml, Encoding.UTF8,
            '                                        Sub(req As HttpWebRequest)
            '                                            req.Method = "POST"
            '                                            req.Host = "converter.telerik.com"
            '                                            req.ContentType = "application/soap+xml; charset=utf-8"
            '                                            req.KeepAlive = False
            '                                        End Sub)
            '
            '            Dim ns As XNamespace = result.Root.Name.Namespace
            '            Dim el As XElement = result.Element(ns + "Envelope").Element(ns + "Body")
            '            Dim resultCode As String = el.Value
            '
            '            If Not String.IsNullOrWhiteSpace(resultCode) AndAlso
            '               Not resultCode.TrimStart(ControlChars.Lf).StartsWith("CONVERSION ERROR", StringComparison.OrdinalIgnoreCase) Then
            '
            '                Dim footer As String = String.Empty
            '
            '                Select Case converterMethod
            '
            '                    Case TelerikCodeConverterMethod.VisualBasicToCSharp
            '                        footer =
            '"//=======================================================
            '//Service provided by Telerik (www.telerik.com)
            '//Conversion powered by Refactoring Essentials.
            '//Twitter: @telerik
            '//Facebook: facebook.com/telerik
            '//======================================================="
            '
            '                    Case TelerikCodeConverterMethod.CSharpToVisualBasic
            '                        footer =
            '"'=======================================================
            ''Service provided by Telerik (www.telerik.com)
            ''Conversion powered by Refactoring Essentials.
            ''Twitter: @telerik
            ''Facebook: facebook.com/telerik
            ''======================================================="
            '
            '                End Select
            '
            '                footer = footer.Replace(Environment.NewLine, ControlChars.Lf)
            '                resultCode = resultCode.Remove(resultCode.Length - footer.Length - 1)
            '            End If
            '
            'Return resultCode.Trim(ControlChars.Lf)

        End Function

#End Region

        Friend NotInheritable Class ConversionResponse
            Public Property ConversionOK As Boolean
            Public Property ConvertedCode As String
            Public Property ErrorMessage As String
        End Class

    End Class

End Namespace

#End Region
