using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CryptoLib.Models;
using CryptoLib.Services;
using DsaWpfApp.Infra;

namespace DsaWpfApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly IDsaService _dsaService;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        // ========== PHẦN SINH KHÓA ==========

        public ObservableCollection<int> KeySizes { get; } =
            new ObservableCollection<int> { 1024, 2048, 3072 };

        private int _selectedKeySize;
        public int SelectedKeySize
        {
            get => _selectedKeySize;
            set { _selectedKeySize = value; OnPropertyChanged(); }
        }

        private string _publicKeyText = "";
        public string PublicKeyText
        {
            get => _publicKeyText;
            set { _publicKeyText = value; OnPropertyChanged(); }
        }

        private string _privateKeyText = "";
        public string PrivateKeyText
        {
            get => _privateKeyText;
            set { _privateKeyText = value; OnPropertyChanged(); }
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private DsaKeyPair? _currentKey;

        // ========== PHẦN KÝ VĂN BẢN ==========

        private string _signInputText = "";
        public string SignInputText
        {
            get => _signInputText;
            set { _signInputText = value; OnPropertyChanged(); }
        }

        private string _signatureBase64 = "";
        public string SignatureBase64
        {
            get => _signatureBase64;
            set { _signatureBase64 = value; OnPropertyChanged(); }
        }

        // ========== PHẦN VERIFY CHỮ KÝ ==========

        private string _verifyInputText = "";
        public string VerifyInputText
        {
            get => _verifyInputText;
            set { _verifyInputText = value; OnPropertyChanged(); }
        }

        private string _verifySignatureBase64 = "";
        public string VerifySignatureBase64
        {
            get => _verifySignatureBase64;
            set { _verifySignatureBase64 = value; OnPropertyChanged(); }
        }

        private string _verifyResultMessage = "Chưa kiểm tra";
        public string VerifyResultMessage
        {
            get => _verifyResultMessage;
            set { _verifyResultMessage = value; OnPropertyChanged(); }
        }

        private bool? _isSignatureValid = null;
        public bool? IsSignatureValid
        {
            get => _isSignatureValid;
            set { _isSignatureValid = value; OnPropertyChanged(); }
        }

        // ========== COMMANDS ==========

        public ICommand GenerateKeyCommand { get; }
        public ICommand SignCommand { get; }
        public ICommand VerifyCommand { get; }

        // ========== CONSTRUCTOR ==========

        public MainViewModel(IDsaService dsaService)
        {
            _dsaService = dsaService ?? throw new ArgumentNullException(nameof(dsaService));

            SelectedKeySize = 2048;

            GenerateKeyCommand = new RelayCommand(_ => GenerateKey());
            SignCommand = new RelayCommand(_ => Sign(), _ => CanSign());
            VerifyCommand = new RelayCommand(_ => Verify(), _ => CanVerify());
        }

        // ----- Sinh khóa -----
        private void GenerateKey()
        {
            try
            {
                _currentKey = _dsaService.GenerateKey(SelectedKeySize);

                PublicKeyText = Convert.ToBase64String(_currentKey.PublicKey);
                PrivateKeyText = Convert.ToBase64String(_currentKey.PrivateKey);

                StatusMessage = $"Đã sinh khóa DSA {SelectedKeySize} bit lúc {DateTime.Now:T}";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi sinh khóa: " + ex.Message;
            }
        }

        // ----- Ký văn bản -----
        private bool CanSign()
            => _currentKey != null && !string.IsNullOrWhiteSpace(SignInputText);

        private void Sign()
        {
            if (_currentKey == null)
            {
                StatusMessage = "Chưa có khóa, hãy Generate Key trước.";
                return;
            }

            try
            {
                var sig = _dsaService.Sign(SignInputText, _currentKey);
                SignatureBase64 = Convert.ToBase64String(sig);

                StatusMessage = $"Đã ký văn bản lúc {DateTime.Now:T}";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi ký: " + ex.Message;
            }
        }

        // ----- Verify chữ ký -----
        private bool CanVerify()
            => _currentKey != null
               && !string.IsNullOrWhiteSpace(VerifyInputText)
               && !string.IsNullOrWhiteSpace(VerifySignatureBase64);

        private void Verify()
        {
            if (_currentKey == null)
            {
                StatusMessage = "Chưa có khóa, hãy Generate Key trước.";
                return;
            }

            byte[] sigBytes;
            try
            {
                sigBytes = Convert.FromBase64String(VerifySignatureBase64);
            }
            catch
            {
                VerifyResultMessage = "Chữ ký không phải Base64 hợp lệ.";
                IsSignatureValid = null;
                StatusMessage = "Lỗi: Signature Base64 không hợp lệ.";
                return;
            }

            try
            {
                var publicKeyOnly = new DsaKeyPair
                {
                    PublicKey = _currentKey.PublicKey,
                    KeySize = _currentKey.KeySize
                };

                bool ok = _dsaService.Verify(VerifyInputText, sigBytes, publicKeyOnly);

                IsSignatureValid = ok;
                VerifyResultMessage = ok ? "Chữ ký HỢP LỆ" : "Chữ ký KHÔNG hợp lệ";
                StatusMessage = "Kết quả verify: " + (ok ? "Hợp lệ" : "Không hợp lệ");
            }
            catch (Exception ex)
            {
                VerifyResultMessage = "Lỗi verify: " + ex.Message;
                IsSignatureValid = null;
                StatusMessage = "Lỗi verify: " + ex.Message;
            }
        }
    }
}
