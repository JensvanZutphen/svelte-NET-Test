import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto';
import { createAddHookMessageChannel } from 'import-in-the-middle';
import { register } from 'node:module';

const { registerOptions } = createAddHookMessageChannel();
register('import-in-the-middle/hook.mjs', import.meta.url, registerOptions);

const defaultExporterEndpoint = 'http://jaeger:4318/v1/traces';
const traceExporter = new OTLPTraceExporter({
	url: process.env.OTEL_EXPORTER_OTLP_ENDPOINT ?? defaultExporterEndpoint
});

const sdk = new NodeSDK({
	serviceName: process.env.OTEL_SERVICE_NAME ?? 'my-sveltekit-client',
	traceExporter,
	instrumentations: [getNodeAutoInstrumentations()]
});

sdk.start();
