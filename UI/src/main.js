import {
	createApp
} from 'vue';
import App from './App.vue';
import store from './plugins/vue-plugins/store/store';
import router from './plugins/vue-plugins/router/router';

// 已在vite.config.js中使用了按需加载，这里只是引入css
import 'element-plus/dist/index.css';
import 'element-plus/es/components/message/style/css';
import 'vxe-table/lib/style.css';

const app = createApp(App);
// 添加middleware，存在路由刷新问题，需要在router之前引用
import middleware from './plugins/middleware/index';
// Element-Plus 设置部分全局工具
import element from './plugins/js/element-ui';
// 引入vxe-table, 做为整个框架的模块化处理插件
import vxetable from './plugins/js/vxe-table';
// 添加utils
import utils from './utils/index';
// 添加自定义全局组件
import packages from './plugins/package/index'
app
	.use(middleware)
	.use(store)
	.use(router)
	.use(element)
	.use(vxetable)
	.use(utils)
	.use(packages)
	.mount('#app');