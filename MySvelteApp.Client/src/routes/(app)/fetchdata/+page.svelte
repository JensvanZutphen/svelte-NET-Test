<script lang="ts">
    import { onMount } from "svelte";
    import { getWeatherForecast } from "$api/schema/sdk.gen";
    import type { WeatherForecast } from "$api/schema/types.gen";
  
    let forecasts: WeatherForecast[] = [];
  
    onMount(async () => {
      try {
        const response = await getWeatherForecast();
        if (response.data) {
          forecasts = response.data;
        } else {
          console.error("Failed to fetch weather data:", response.error);
          forecasts = [];
        }
      } catch (error) {
        console.error("Failed to fetch or validate weather data:", error);
        forecasts = [];
      }
    });
  </script>
  

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from the server.</p>

{#if forecasts.length === 0}
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
