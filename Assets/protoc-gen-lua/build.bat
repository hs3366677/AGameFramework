rem �л���.protoЭ�����ڵ�Ŀ¼  
cd  protobuf\luascript  
rem ����ǰ�ļ����е�����Э���ļ�ת��Ϊlua�ļ�  
for %%i in (*.proto) do (    
echo %%i  
"..\..\protoc.exe" --plugin=protoc-gen-lua="..\..\plugin\protoc-gen-lua.bat" --lua_out=. %%i  
  
)  
echo end