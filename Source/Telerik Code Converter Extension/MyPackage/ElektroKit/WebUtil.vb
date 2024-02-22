




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
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Linq

#End Region

#Region " DevCase.Core.NET.WebUtil "

Namespace DevCase.Core.NET

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains related utilities to the World Wide Web.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class WebUtil

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="WebUtil"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asynchronously posts an XML HTTP request to the specified server with a custom <see cref="Encoding"/>, and returns the response.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="uri">
        ''' The server <see cref="Uri"/>.
        ''' </param>
        ''' 
        ''' <param name="xml">
        ''' A <see cref="XDocument"/> that represents the XML content to POST to the server.
        ''' </param>
        ''' 
        ''' <param name="enc">
        ''' The text encoding.
        ''' </param>
        ''' 
        ''' <param name="request">
        ''' A <see cref="HttpWebRequest"/> object that represents the request parameters and custom header values. 
        ''' <para></para>
        ''' (It is not necessary to set the <see cref="HttpWebRequest.Method"/> and <see cref="HttpWebRequest.ContentLength"/> properties)
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting response returned by the server.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Async Function PostXmlHttpRequestAsync(ByVal uri As Uri, ByVal xml As XDocument, ByVal enc As Encoding, ByVal request As Action(Of HttpWebRequest)) As Task(Of XDocument)

            Return Await WebUtil.PostXmlHttpRequestAsync(uri, xml.ToString(), enc, request)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asynchronously posts an XML HTTP request to the specified server with a custom <see cref="Encoding"/>, and returns the response.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="uri">
        ''' The server <see cref="Uri"/>.
        ''' </param>
        ''' 
        ''' <param name="xmlContent">
        ''' The XML content to POST to the server.
        ''' </param>
        ''' 
        ''' <param name="enc">
        ''' The text encoding.
        ''' </param>
        ''' 
        ''' <param name="request">
        ''' A <see cref="HttpWebRequest"/> object that represents the request parameters and custom header values. 
        ''' <para></para>
        ''' (It is not necessary to set the <see cref="HttpWebRequest.Method"/> and <see cref="HttpWebRequest.ContentLength"/> properties)
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting response returned by the server.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException"></exception>
        ''' <exception cref="WebException"></exception>
        ''' <exception cref="Exception"></exception>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Async Function PostXmlHttpRequestAsync(ByVal uri As Uri, ByVal xmlContent As String, ByVal enc As Encoding, ByVal request As Action(Of HttpWebRequest)) As Task(Of XDocument)

            If (uri Is Nothing) Then
                Throw New ArgumentNullException(paramName:="uri")
            End If

            If (String.IsNullOrWhiteSpace(xmlContent)) Then
                Throw New ArgumentNullException(paramName:="xmlContent")
            End If

            If (enc Is Nothing) Then
                Throw New ArgumentNullException(paramName:="enc")
            End If

            If (request Is Nothing) Then
                Throw New ArgumentNullException(paramName:="request")
            End If

            Dim result As String = String.Empty
            Dim content As Byte() = enc.GetBytes(xmlContent)
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create(uri), HttpWebRequest)
            ' Set default value for HttpWebRequest.ContentType property.
            req.ContentType = String.Format("application/xml; charset={0}", enc.WebName.ToLower())
            ' Set and/or replace custom HttpWebRequest properties.
            request.Invoke(req)
            ' Override HttpWebRequest.Method and HttpWebRequest.ContentLength values.
            req.Method = "POST"
            req.ContentLength = content.Length

            Try
                ' Write XML content.
                Using reqStream As Stream = Await req.GetRequestStreamAsync()
                    Await reqStream.WriteAsync(content, 0, content.Length)
                    reqStream.Close()
                End Using

                ' Send XML content.
                Using res As HttpWebResponse = DirectCast(req.GetResponse(), HttpWebResponse)
                    If (res.StatusCode = HttpStatusCode.OK) Then
                        ' Get server response.
                        Using resStream As Stream = res.GetResponseStream()
                            Using reader As New StreamReader(resStream, enc)
                                result = Await reader.ReadToEndAsync()
                            End Using
                        End Using
                    End If
                End Using

            Catch ex As WebException
                Throw

            Catch ex As Exception
                Throw

            End Try

            Return XDocument.Parse(result, LoadOptions.None)

        End Function

#End Region

    End Class

End Namespace

#End Region
