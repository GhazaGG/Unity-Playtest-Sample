using Unity.Collections;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace PlaytestSample
{
    // Test switch: delays every packet this peer sends, so two peers on one machine behave like a real connection.
    // Set SendDelayMs before starting the session, or start a build with "-latency <ms>". Each peer adds its own
    // delay; measure the result with NetworkTransport.GetCurrentRtt (30 on both peers read about 140 ms here).
    public class LatencySimulator : MonoBehaviour, INetworkStreamDriverConstructor
    {
        [SerializeField, Min(0)] private int _sendDelayMs;

        public int SendDelayMs
        {
            get => _sendDelayMs;
            set => _sendDelayMs = Mathf.Max(0, value);
        }

        private void Awake()
        {
            var args = System.Environment.GetCommandLineArgs();
            var i = System.Array.IndexOf(args, "-latency");
            if (i >= 0 && i + 1 < args.Length && int.TryParse(args[i + 1], out var ms)) SendDelayMs = ms;
            UnityTransport.s_DriverConstructor = this;
        }

        public void CreateDriver(UnityTransport transport, out NetworkDriver driver, out NetworkPipeline unreliableFragmentedPipeline,
            out NetworkPipeline unreliableSequencedFragmentedPipeline, out NetworkPipeline reliableSequencedPipeline)
        {
            if (_sendDelayMs == 0)
            {
                transport.CreateDriver(transport, out driver, out unreliableFragmentedPipeline, out unreliableSequencedFragmentedPipeline, out reliableSequencedPipeline);
                return;
            }

            var settings = transport.GetDefaultNetworkSettings();
            settings.WithSimulatorStageParameters(maxPacketCount: 1000, mode: ApplyMode.SentPacketsOnly, packetDelayMs: _sendDelayMs);
            driver = NetworkDriver.Create(new UDPNetworkInterface(), settings);
            transport.GetDefaultPipelineConfigurations(out var unreliable, out var sequenced, out var reliable);
            unreliableFragmentedPipeline = driver.CreatePipeline(WithSimulator(unreliable));
            unreliableSequencedFragmentedPipeline = driver.CreatePipeline(WithSimulator(sequenced));
            reliableSequencedPipeline = driver.CreatePipeline(WithSimulator(reliable));
        }

        private static NativeArray<NetworkPipelineStageId> WithSimulator(NativeArray<NetworkPipelineStageId> stages)
        {
            var result = new NativeArray<NetworkPipelineStageId>(stages.Length + 1, Allocator.Temp);
            NativeArray<NetworkPipelineStageId>.Copy(stages, result, stages.Length);
            result[stages.Length] = NetworkPipelineStageId.Get<SimulatorPipelineStage>();
            return result;
        }
    }
}
