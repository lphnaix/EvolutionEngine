Python 工具环境
================

步骤
1. 进入 tools 目录：`cd tools`
2. 创建虚拟环境：`python -m venv env`
3. 激活：
   - PowerShell: `.\env\Scripts\Activate.ps1`
   - cmd: `env\Scripts\activate.bat`
4. 安装依赖：`pip install -r requirements.txt`（包含 pyyaml，用于读写 JSON/YAML）

结构
- env/            # 虚拟环境（不提交，可本地创建）
- common/         # 数据模型、SQLite 助手
- requirements.txt
