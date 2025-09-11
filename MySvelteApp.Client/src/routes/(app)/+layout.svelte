<script lang="ts">
  import '$src/app.css';
  import * as Sidebar from "$lib/components/ui/sidebar/index.js";
  import AppSidebar from "$lib/components/app-sidebar.svelte";
  let { children } = $props();
</script>

<Sidebar.Provider>
  <AppSidebar />
  <Sidebar.Inset>
    <header class="flex h-16 shrink-0 items-center gap-2 px-4">
      <Sidebar.Trigger class="-ml-1" />
      <div class="flex items-center space-x-3">
        <img src="/favicon.png" alt="Logo" class="w-8 h-8 rounded-3xl" />
        <span class="text-xl font-bold text-gray-800">Svelte-NET Demo</span>
      </div>
    </header>
    <div class="flex flex-1 flex-col gap-4 p-4">
      {@render children?.()}
    </div>
  </Sidebar.Inset>
</Sidebar.Provider>
