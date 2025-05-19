using Mirror; // Cần thêm dòng này để sử dụng các tính năng của Mirror
using UnityEngine; // Cần cho các tính năng cơ bản của Unity

// Lớp script này kế thừa từ NetworkBehaviour để hoạt động với Mirror
public class NetworkSwitchProto : NetworkBehaviour
{
    // [SyncVar] là thuộc tính của Mirror.
    // Nó đánh dấu biến 'isOn' cần được đồng bộ hóa từ Server đến tất cả Clients.
    // Khi Server thay đổi giá trị của 'isOn', Mirror sẽ tự động gửi giá trị mới đến tất cả Clients.
    // 'hook = nameof(OnStateChanged)' nghĩa là khi giá trị của 'isOn' thay đổi trên Client,
    // hàm 'OnStateChanged' sẽ được gọi. Điều này giúp chúng ta phản ứng lại sự thay đổi (ví dụ: đổi màu).
    [SyncVar(hook = nameof(OnStateChanged))]
    public bool isOn = false; // Biến lưu trạng thái của công tắc (Bật/Tắt)

    private SpriteRenderer spriteRenderer; // Tham chiếu đến component SpriteRenderer để đổi màu

    // Hàm Awake được gọi một lần khi script được tạo ra
    void Awake()
    {
        // Lấy component SpriteRenderer từ cùng GameObject này
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Đặt màu ban đầu dựa trên trạng thái 'isOn' (trước khi đồng bộ hoàn toàn, nó sẽ dùng giá trị mặc định)
        UpdateColor();
    }

    // Hàm này là 'hook' cho SyncVar 'isOn'.
    // Nó được gọi trên TẤT CẢ Clients (bao gồm cả Host Client)
    // mỗi khi giá trị của 'isOn' thay đổi (do Server gửi đến).
    void OnStateChanged(bool oldState, bool newState)
    {
        Debug.Log($"Switch state changed from {oldState} to {newState}");
        // Cập nhật màu sắc dựa trên trạng thái mới
        UpdateColor();
    }

    // Hàm nội bộ để cập nhật màu sắc của Sprite dựa trên trạng thái 'isOn'
    void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            // Nếu isOn là true, đặt màu xanh lá cây; ngược lại, đặt màu đỏ.
            spriteRenderer.color = isOn ? Color.green : Color.red;
        }
    }

    // Hàm này sẽ được gọi khi người chơi tương tác (ví dụ: click chuột)
    // [Command] là thuộc tính của Mirror.
    // Nó đánh dấu hàm này chỉ được gọi từ Client, nhưng sẽ được THỰC THI trên Server.
    // Đây là nguyên tắc "Server Authority" (Server có quyền quyết định cuối cùng).
    // Client yêu cầu Server thực hiện hành động (CmdToggleSwitch).
    // Server thực hiện hành động (thay đổi isOn).
    // Mirror tự động đồng bộ sự thay đổi của isOn (SyncVar) về lại tất cả Clients.
    // requiresAuthority = false cho phép bất kỳ client nào gọi command này,
    // ngay cả khi họ không có quyền sở hữu (authority) đối với vật thể switch này.
    [Command(requiresAuthority = false)]
    public void CmdToggleSwitch()
    {
        // Code bên trong hàm [Command] này CHỈ chạy trên Server.
        // Thay đổi giá trị của SyncVar 'isOn'.
        // Sự thay đổi này sẽ tự động được Mirror đồng bộ đến Clients.
        isOn = !isOn;
        Debug.Log($"Server toggled switch to: {isOn}");
    }

    // Hàm OnMouseDown của Unity được gọi khi người chơi click chuột vào Collider 2D của vật thể này.
    // Hàm này chạy trên client cục bộ.
    void OnMouseDown()
    {
        // Debug.Log("Mouse clicked on switch");

        // Kiểm tra xem đây có phải là một client đang hoạt động không.
        // Chúng ta chỉ muốn gọi Command lên Server từ một client.
        if (isClient)
        {
            // Gọi hàm Command trên Server.
            // Client cục bộ yêu cầu Server thực hiện CmdToggleSwitch().
            CmdToggleSwitch();
        }
    }
}