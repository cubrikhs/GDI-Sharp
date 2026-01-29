# GDI-Sharp

GDI-Sharp is a C# rendering playground that demonstrates **CPU-based software rendering** using GDI+ and **GPU-based post-processing** using Direct3D 11 (via SharpDX).  
The project is structured like a small graphics engine, inspired by how real-world render pipelines and shader systems work, but simplified for learning and experimentation.

The core idea is:
- **CPU path**: manually write pixels into memory and display them with GDI.
- **GPU path**: capture the desktop, feed it into Direct3D, and apply HLSL shaders in real time.

This repository is useful for understanding how low-level rendering works without relying on high-level engines like Unity or Unreal.
