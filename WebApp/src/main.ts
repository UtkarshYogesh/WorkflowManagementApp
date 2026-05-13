import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import "./style.css";
import { abilitiesPlugin } from "@casl/vue";
import { ability } from "./permissions/ability";

import { VueQueryPlugin, QueryClient } from "@tanstack/vue-query";

const queryClient = new QueryClient();

const app = createApp(App);

app.use(router);
app.use(VueQueryPlugin, { queryClient });
app.use(abilitiesPlugin, ability);

app.mount("#app");
