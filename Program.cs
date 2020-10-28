using System;
using System.IO;

namespace DartDeserialize
{
    class Program
    {
        static void Main(string[] args)
        { 
            string file = @"E:\BaiduNetdiskDownload\Easy_Flutter\151abe691959ea9e037dcbe2ea2ea557\easy_flutter\lib\armeabi-v7a\libapp.so";
            file = @"E:\BaiduNetdiskDownload\Easy_Flutter\151abe691959ea9e037dcbe2ea2ea557\easy_flutter\lib\arm64-v8a\libapp.so";
            FileStream fs = new FileStream(file, FileMode.Open);

            SerializedDartReaderElf dr = new SerializedDartReaderElf(fs);
            (Snapshot vm, Snapshot isolate, TargetId target) = dr.GetVMIsolateSnapshotAndTarget();

            DartEnv env = new DartEnv(target);

            SnapshotReader vmReader = new SnapshotReaderVM(vm);
            vmReader.ResolveSnapshot(env);

            SnapshotReader isolateReader = new SnapshotReaderIsolate(isolate);
            isolateReader.ResolveSnapshot(env);
        }
    }
}
