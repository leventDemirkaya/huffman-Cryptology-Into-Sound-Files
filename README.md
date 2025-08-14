# 🎵 Audio Steganography - Hide Secret Messages in Audio Files

![.NET](https://img.shields.io/badge/.NET-6.0-blue)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

> ✨ **Description:**  
> This project uses **C# .NET** and the **LSB (Least Significant Bit)** method to hide secret text, image, and video files inside **.wav** audio files.  
> The original audio quality is preserved while securely and invisibly embedding the messages.  
> Additionally, Huffman coding is used to compress and secure the hidden data.

## 📑 Contents  
- [📜 About the Project](#about-the-project)  
- [⚡ Features](#features)  
- [🧩 Algorithm and Techniques](#algorithm-and-techniques)  
- [⚙️ Installation and Usage](#installation-and-usage)  
- [📊 Sample Output](#sample-output)  
- [🛠 Technologies](#technologies)  
- [🤝 Contributing](#contributing)  
- [📄 License](#license)  
- [📬 Contact](#contact)

<a id="about-the-project"></a>
## 📜 About the Project  
This project is an audio steganography application that hides secret messages by manipulating the least significant bits of audio files.  
Different file types such as text, image, and video are converted to binary and embedded into the audio file using the Fibonacci sequence and LSB method.  
Huffman coding is also applied to compress and secure the data.

<a id="features"></a>
## ⚡ Features  
- ✅ Secure and invisible message hiding using the LSB method  
- ✅ Support for embedding text, image, and video files into audio  
- ✅ Bit placement using the Fibonacci sequence  
- ✅ Data compression and security with Huffman coding  
- ✅ Support for Turkish characters  
- ✅ Management of original and stego audio files  
- ✅ User-friendly Windows Forms interface  
- ✅ Binary conversion and processing of audio, image, and video files  

<a id="algorithm-and-techniques"></a>
## 🧩 Algorithm and Techniques  
1. Convert the audio file to a byte array and then to binary format.  
2. Convert the secret file (text, image, or video) to binary format.  
3. Embed message bits into specific bits of the audio file using the Fibonacci sequence.  
4. Compress and encode the hidden data using Huffman coding.  
5. Convert the modified binary data back to bytes.  
6. Create a new .wav file containing the hidden message, playable as audio.  
7. Extract and decode the hidden message from the audio file using Huffman codes.

<a id="installation-and-usage"></a>
## ⚙️ Installation and Usage  
1. 📥 Clone the repository:  
   ```bash
   git clone https://github.com/leventDemirkaya/audio-steganography.git
   cd audio-steganography
2. 💻 Open the project in Visual Studio or run it using the dotnet CLI.
3. 🎵 Use the program interface to select a .wav file, choose the secret text, image, or video file to embed, and embed the message.
4. 🔊 Listen to the generated audio file or extract the hidden message to verify.

<a id="sample-output"></a>
## 📊 Sample Output
### 🎙️ Selected Audio File:
example.wav

### 📝 Secret Message:
"Hello, this is a secret message."

### 🔢 Binary Message Parts:
01001101 01100101 01110010 01101000 01100001 01100010 01100001 ...

### 🎧 Generated Audio File:
hidden1.wav

### 📬 Extracted Message:
"Hello, this is a secret message."

<a id="technologies"></a>
## 🛠 Technologies
- 💻 C#
- 🖥 .NET Framework / .NET 6.0
- 🎨 Windows Forms
- 🎵 NAudio library

<a id="contributing"></a>
## 🤝 Contributing
💡 Contributions are very welcome!
- 🐛 Report issues via the Issues tab.
- 🚀 Submit pull requests for improvements.

<a id="license"></a>
## 📄 License
📝 This project is licensed under the MIT License. See the LICENSE file for details.

<a id="contact"></a>
## 📬 Contact
📧 leventdemirkaya@outlook.com
