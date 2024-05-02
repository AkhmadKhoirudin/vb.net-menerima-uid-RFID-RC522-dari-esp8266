Public Class Form3
    Private isServerRunning As Boolean = False ' Untuk menyimpan status server (berjalan atau tidak)
    Private lastSuccessfulUID As String = ""
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Menambahkan event handler secara manual setelah membuat instance dari modul
        AddHandler ESPModule.MessageReceived, AddressOf ESPMessageReceivedHandler
    End Sub

    Private Sub ESPMessageReceivedHandler(message As String)
        ' Mendapatkan UID dari pesan
        Dim receivedUID As String = message.Trim()

        ' Memeriksa apakah UID sama dengan UID terakhir yang berhasil dikirim
        If receivedUID <> lastSuccessfulUID Then
            ' Jika berbeda, update lastSuccessfulUID dan label
            lastSuccessfulUID = receivedUID

            ' Mengatur teks label dengan UID terbaru yang berhasil dikirim
            Label1.Invoke(Sub() Label1.Text = $" {receivedUID}")
        Else
            ' Jika UID sama dengan UID terakhir yang berhasil dikirim, tidak perlu melakukan apa-apa
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If isServerRunning Then
            ' Jika server sedang berjalan, hentikan mendengarkan koneksi
            ESPModule.StopListening()
            isServerRunning = False
            Button1.Text = "Start Listening" ' Ubah teks tombol menjadi "Mulai Mendengarkan"
        Else
            ' Jika server tidak sedang berjalan, mulai mendengarkan koneksi
            ESPModule.StartListening(12345)
            isServerRunning = True
            Button1.Text = "Stop Listening" ' Ubah teks tombol menjadi "Hentikan Mendengarkan"
        End If
    End Sub
End Class
