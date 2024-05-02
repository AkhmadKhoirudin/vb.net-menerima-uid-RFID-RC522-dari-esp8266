Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Module ESPModule
    Private server As TcpListener
    Private cancellationTokenSource As New CancellationTokenSource()

    ' Definisikan sebuah event untuk memberi tahu bahwa pesan baru telah diterima
    Public Event MessageReceived(message As String)

    ' Metode untuk memulai mendengarkan koneksi dari ESP
    Public Sub StartListening(port As Integer)
        server = New TcpListener(IPAddress.Any, port)
        server.Start()
        Console.WriteLine($"Server berjalan di semua antarmuka pada port {port}")

        Dim listenerThread As New Thread(AddressOf ListenForConnections)
        listenerThread.Start()
    End Sub

    ' Metode untuk menghentikan mendengarkan koneksi dari ESP
    Public Sub StopListening()
        cancellationTokenSource.Cancel()
        server.Stop()
        Console.WriteLine("Server berhenti.")
    End Sub

    ' Metode untuk mendengarkan koneksi dari ESP
    Private Sub ListenForConnections()
        While Not cancellationTokenSource.IsCancellationRequested
            Try
                Dim client As TcpClient = server.AcceptTcpClient()
                Console.WriteLine("Klien terhubung.")

                Dim stream As NetworkStream = client.GetStream()
                Dim buffer(1024) As Byte
                Dim byteCount As Integer
                byteCount = stream.Read(buffer, 0, buffer.Length)
                Dim message As String = Encoding.ASCII.GetString(buffer, 0, byteCount)

                ' Picu event MessageReceived dengan pesan yang diterima sebagai argumen
                RaiseEvent MessageReceived(message)

                client.Close()
            Catch ex As Exception
                Console.WriteLine($"Terjadi kesalahan: {ex.Message}")
            End Try
        End While
    End Sub
End Module
