const userModel = firebase.auth();
const db = firebase.firestore();

const app = Sammy('#root' , function() {
    this.use('Handlebars','hbs');
    //Home routes
    this.get('/home',function(context){
        db.collection('movies').get()
        .then((res) => {
            context.movies = res.docs.map((movie) =>{ return {id: movie.id, ...movie.data()}});
            extendCtx(context)
            .then(function(){
                this.partial('/templates/home.hbs');
            })
        })
        .catch(errorHandler);
        
    });
    //User routes
    //get-login
    this.get('/login',function(context){
        extendCtx(context)
        .then(function(){
            this.partial('/templates/login.hbs');
        })
    })
    //post-login
    this.post('/login',function(context){
        const { email, password } = context.params;

        userModel.signInWithEmailAndPassword(email,password)
        .then((userData) => {
            saveUserData(userData);
            console.log(userData);
            this.redirect('#/home');
        })
        .catch(errorHandler);
    })
    //get-register
    this.get('/register',function(context){
        extendCtx(context)
        .then(function(){
            this.partial('/templates/register.hbs');
        })
    })
    // post-register
    this.post('/register',function(context){
        const {email , password, repeatPass } = context.params;

        if (password !== repeatPass) {
            return;
        }

        userModel.createUserWithEmailAndPassword(email, password)
        .then((userData) => {
            console.log(userData);
            this.redirect('#/home');
        })
        .catch(errorHandler);

    });

    this.get('/logout',function(context){
        userModel.signOut()
        .then((res) => {
            clearUserData();
            this.redirect('#/login')
        })
        .catch(errorHandler);
   });

    //Movies routes
    this.get('/add-movie', function(context){
        extendCtx(context)
        .then(function(){
            this.partial('/templates/addMovie.hbs');
        })
    })    
    this.post('/add-movie',function(context){
    const { title, imageUrl ,description } = context.params;
    
      db.collection('movies').add({
        title,
        description,
        imageUrl,
        creator: getUserData().uid,
        likes: [],
      })
      .then((createdProducts) =>{
          console.log(createdProducts);
          this.redirect('#/home');
      })
      .catch(errorHandler);
    });

    this.get('/edit-movie', function(context){
        extendCtx(context)
        .then(function(){
            this.partial('/templates/editMovie.hbs');
        })
    })    
    this.get('/details/:id', function(context){
        
        const { id } = context.params;
        console.log(id);
        db.collection('movies').doc(id).get()
        .then((res) => {
            const data = res.data();
            const { uid } = getUserData();
            
            const imTheCreator = data.creator === uid;
            
            const index = data.likes.indexOf(uid);

            const imInLikesList = index > -1;

            const movieLikes = data.likes.length;
            console.log(data);
            context.movie = { ...data, imTheCreator, id:id, imInLikesList ,movieLikes };
            extendCtx(context)
            .then(function(){
                this.partial('/templates/details.hbs');
            })
        });  
    })    
    
    this.get('/delete/:id', function(context){
        const { id } = context.params;

        db.collection('movies').doc(id).delete()
        .then(() =>{
            this.redirect('#/home')
        })
        .catch(errorHandler)
    });
    
    this.get('/edit/:id',function(context){
        const { id } = context.params;
        db.collection('movies')
        .doc(id)
        .get()
        .then((res) => {
            console.log(res.data());
            context.movie = { id:id, ...res.data() };

            extendCtx(context)
            .then(function() {
                this.partial('./templates/editMovie.hbs');
            });
        })
    });

    this.post('/edit/:id', function(context) {
        const { id , title, description , imageUrl } = context.params;

        db.collection('movies')
        .doc(id)
        .get()
        .then((res) => {
            return db.collection('movies')
            .doc(id)
            .set({
                ...res.data(),
                title,
                description,
                imageUrl
            })
        })
        .then((res) =>{
            this.redirect(`#/details/${id}`)
        })
        .catch(errorHandler);

    })

    this.get('/like/:id' , function(context) {
       const { id } = context.params;
       const { uid } = getUserData();
       
       db.collection('movies')
       .doc(id)
       .get()
       .then((res) => {
           const movieData = { ...res.data() };
           movieData.likes.push(uid);

           return db.collection('movies')
           .doc(id)
           .set(movieData);
       })
       .then(() => {
           this.redirect(`#/details/${id}`);
       })
       .catch(errorHandler);
    });
});

(() => {
    app.run('/home');
})();

function extendCtx(context) {
    const user = getUserData();

    context.isLoggedIn = Boolean(user);
    context.email = user ? user.email: '';

    return context.loadPartials({
        'header': './partials/header.hbs',
        'footer': './partials/footer.hbs'
    });
}

function errorHandler(error){
    console.log(error);
}


//User data functions
function saveUserData(data){
    const { user: { email,uid } } = data;
     localStorage.setItem('user', JSON.stringify({email , uid}));
}

function getUserData(){
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
}

function clearUserData(){
    this.localStorage.removeItem('user');
}