import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";




const RandomPokemon = z.object({ name: z.string().nullable(), type: z.string().nullable(), image: z.string().nullable() }).partial();
const WeatherForecast = z.object({ date: z.string(), temperatureC: z.number().int(), summary: z.string().nullable(), temperatureF: z.number().int() }).partial();

export const schemas = {
	RandomPokemon,
	WeatherForecast,
};

const endpoints = makeApi([
	{
		method: "get",
		path: "/RandomPokemon",
		alias: "getRandomPokemon",
		requestFormat: "json",
		response: RandomPokemon,
	},
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
