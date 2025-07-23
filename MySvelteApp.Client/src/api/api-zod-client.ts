import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";




const WeatherForecast = z.object({ temperatureC: z.number().int(), summary: z.string().nullable(), temperatureF: z.number().int(), date: z.string() }).partial();

export const schemas = {
	WeatherForecast,
};

const endpoints = makeApi([
	{
		method: "get",
		path: "/WeatherForecast",
		alias: "getWeatherForecast",
		requestFormat: "json",
		response: z.array(WeatherForecast),
	},
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
    return new Zodios(baseUrl, endpoints, options);
}
