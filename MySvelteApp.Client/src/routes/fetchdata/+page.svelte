<script lang="ts">
    import { onMount } from "svelte";
    import { api, schemas } from "$api";
  
    type WeatherForecast = typeof schemas.WeatherForecast._type; // This is getting the type from the Zod schema so you do not need to use api.ts
    let forecasts: WeatherForecast[] = [];
  
    onMount(async () => {
      try {
        // The Zod client already does runtime validation!
        forecasts = await api.getWeatherForecast();
      } catch (error) {
        console.error("Failed to fetch or validate weather data:", error);
        forecasts = [];
      }
    });
  </script>
  

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from the server.</p>

{#if forecasts == null}
    <p><em>Loading...</em></p>
{:else}
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            {#each forecasts as forecast}
                <tr>
                    <td>{new Date(forecast.date ?? "").toLocaleDateString()}</td>
                    <td>{forecast.temperatureC}</td>
                    <td>{forecast.temperatureF}</td>
                    <td>{forecast.summary}</td>
                </tr>
            {/each}
        </tbody>
    </table>
{/if}
